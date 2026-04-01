// StateMachines/PurchaseStateMachine.cs  (complete)

using GamePlatform.Catalog.Contracts;
using GamePlatform.Identity.Contracts;
using GamePlatform.Inventory.Contracts;
using MassTransit;
using Trading.Service.Contracts;
using Trading.Service.StateMachines.Activities;

namespace Trading.Service.StateMachines;

public class PurchaseStateMachine : MassTransitStateMachine<PurchaseState>
{
    private readonly ILogger<PurchaseStateMachine> _logger;

    public State Accepted             { get; private set; } = null!;
    public State ItemPriceCalculated  { get; private set; } = null!;
    public State GilDebited           { get; private set; } = null!;
    public State ItemGranted          { get; private set; } = null!;
    public State Completed            { get; private set; } = null!;
    public State Faulted              { get; private set; } = null!;

    public Event<PurchaseRequested>         PurchaseRequested        { get; private set; } = null!;
    public Event<CatalogItemFound>          CatalogItemFound         { get; private set; } = null!;
    public Event<CatalogItemNotFound>       CatalogItemNotFound      { get; private set; } = null!;
    public Event<GilDebited>                GilDebitedEvent          { get; private set; } = null!;
    public Event<InsufficientFunds>         InsufficientFunds        { get; private set; } = null!;
    public Event<InventoryItemsGranted>     InventoryItemsGranted    { get; private set; } = null!;
    public Event<InventoryItemsSubtracted>  InventoryItemsSubtracted { get; private set; } = null!;
    public Event<GilCredited>              GilCredited              { get; private set; } = null!;

    public PurchaseStateMachine(ILogger<PurchaseStateMachine> logger)
    {
        _logger = logger;

        InstanceState(state => state.CurrentState);
        ConfigureEvents();

        // ── Initially ─────────────────────────────────────────────────────
        Initially(
            When(PurchaseRequested)
                .Then(ctx =>
                {
                    ctx.Saga.UserId      = ctx.Message.UserId;
                    ctx.Saga.ItemId      = ctx.Message.ItemId;
                    ctx.Saga.Quantity    = ctx.Message.Quantity;
                    ctx.Saga.Received    = DateTimeOffset.UtcNow;
                    ctx.Saga.LastUpdated = ctx.Saga.Received;
                    _logger.LogInformation(
                        "[{CorrelationId}] Purchase requested — User: {UserId}, Item: {ItemId}",
                        ctx.Saga.CorrelationId, ctx.Saga.UserId, ctx.Saga.ItemId);
                })
                .Activity(x => x.OfType<GetCatalogItemActivity>())
                .TransitionTo(Accepted));

        // ── During Accepted ───────────────────────────────────────────────
        During(Accepted,
            When(CatalogItemFound)
                .Then(ctx =>
                {
                    ctx.Saga.ItemName      = ctx.Message.Name;
                    ctx.Saga.PurchaseTotal = ctx.Message.Price * ctx.Saga.Quantity;
                    ctx.Saga.LastUpdated   = DateTimeOffset.UtcNow;
                    _logger.LogInformation(
                        "[{CorrelationId}] Catalog item found — {Name} @ {Price}",
                        ctx.Saga.CorrelationId, ctx.Message.Name, ctx.Message.Price);
                })
                .Activity(x => x.OfType<DebitGilActivity>())
                .TransitionTo(ItemPriceCalculated),

            When(CatalogItemNotFound)
                .Then(ctx =>
                {
                    ctx.Saga.ErrorMessage = $"Item {ctx.Saga.ItemId} not found in catalog";
                    ctx.Saga.LastUpdated  = DateTimeOffset.UtcNow;
                    _logger.LogWarning(
                        "[{CorrelationId}] Catalog item not found: {ItemId}",
                        ctx.Saga.CorrelationId, ctx.Saga.ItemId);
                })
                .TransitionTo(Faulted)
                .ThenAsync(async ctx => await ctx.Publish(
                    new PurchaseFailed(ctx.Saga.CorrelationId, ctx.Saga.ErrorMessage!))));

        // ── During ItemPriceCalculated ────────────────────────────────────
        During(ItemPriceCalculated,
            When(GilDebitedEvent)
                .Then(ctx =>
                {
                    ctx.Saga.LastUpdated = DateTimeOffset.UtcNow;
                    _logger.LogInformation(
                        "[{CorrelationId}] Gil debited — Total: {Total}",
                        ctx.Saga.CorrelationId, ctx.Saga.PurchaseTotal);
                })
                .Activity(x => x.OfType<GrantItemsActivity>())
                .TransitionTo(GilDebited),

            When(InsufficientFunds)
                .Then(ctx =>
                {
                    ctx.Saga.ErrorMessage = "User has insufficient funds";
                    ctx.Saga.LastUpdated  = DateTimeOffset.UtcNow;
                    _logger.LogWarning(
                        "[{CorrelationId}] Insufficient funds for user {UserId}",
                        ctx.Saga.CorrelationId, ctx.Saga.UserId);
                })
                .TransitionTo(Faulted)
                .ThenAsync(async ctx => await ctx.Publish(
                    new PurchaseFailed(ctx.Saga.CorrelationId, ctx.Saga.ErrorMessage!))));

        // ── During GilDebited ─────────────────────────────────────────────
        During(GilDebited,
            When(InventoryItemsGranted)
                .Then(ctx =>
                {
                    ctx.Saga.LastUpdated = DateTimeOffset.UtcNow;
                    _logger.LogInformation(
                        "[{CorrelationId}] Inventory items granted — Purchase complete",
                        ctx.Saga.CorrelationId);
                })
                .TransitionTo(Completed)
                .ThenAsync(async ctx => await ctx.Publish(
                    new PurchaseCompleted(ctx.Saga.CorrelationId))),
            // Compensation path — inventory grant failed externally and was subtracted
            When(InventoryItemsSubtracted)
                .Activity(x => x.OfType<CreditGilActivity>()),      // InventoryItemsSubtracted → sends CreditGil

            When(GilCredited)
                .Then(ctx =>
                {
                    ctx.Saga.ErrorMessage = ctx.Saga.ErrorMessage ?? "Inventory grant failed";
                    ctx.Saga.LastUpdated  = DateTimeOffset.UtcNow;
                })
                .TransitionTo(Faulted)
                .ThenAsync(async ctx => await ctx.Publish(
                    new PurchaseFailed(ctx.Saga.CorrelationId, ctx.Saga.ErrorMessage!))));
        
        SetCompletedWhenFinalized();
    }

    private void ConfigureEvents()
    {
        Event(() => PurchaseRequested,       e => e.CorrelateById(m => m.Message.PurchaseId));
        Event(() => CatalogItemFound,        e => e.CorrelateById(m => m.Message.CorrelationId));
        Event(() => CatalogItemNotFound,     e => e.CorrelateById(m => m.Message.CorrelationId));
        Event(() => GilDebitedEvent,         e => e.CorrelateById(m => m.Message.CorrelationId));
        Event(() => InsufficientFunds,       e => e.CorrelateById(m => m.Message.CorrelationId));
        Event(() => InventoryItemsGranted,   e => e.CorrelateById(m => m.Message.CorrelationId));
        Event(() => InventoryItemsSubtracted,e => e.CorrelateById(m => m.Message.CorrelationId));
        Event(() => GilCredited,             e => e.CorrelateById(m => m.Message.CorrelationId));
    }
}