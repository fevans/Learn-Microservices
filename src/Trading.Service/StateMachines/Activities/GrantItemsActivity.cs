using GamePlatform.Identity.Contracts;
using GamePlatform.Inventory.Contracts;
using MassTransit;

namespace Trading.Service.StateMachines.Activities;

public class GrantItemsActivity(ILogger<GrantItemsActivity> logger)
    : IStateMachineActivity<PurchaseState, GilDebited>
{
    public void Accept(StateMachineVisitor visitor) => visitor.Visit(this);
    public void Probe(ProbeContext context) =>
        context.CreateScope(nameof(GrantItemsActivity));

    public async Task Execute(
        BehaviorContext<PurchaseState, GilDebited> context,
        IBehavior<PurchaseState, GilDebited> next)
    {
        logger.LogInformation(
            "[{CorrelationId}] Granting {Quantity}x item {ItemId} to user {UserId}",
            context.Saga.CorrelationId,
            context.Saga.Quantity,
            context.Saga.ItemId,
            context.Saga.UserId);

        await context.Publish(new GrantItems(
            CorrelationId: context.Saga.CorrelationId,
            UserId:        context.Saga.UserId,
            CatalogItemId: context.Saga.ItemId,
            Quantity:      context.Saga.Quantity));

        await next.Execute(context);
    }

    public async Task Faulted<TException>(
        BehaviorExceptionContext<PurchaseState, GilDebited, TException> context,
        IBehavior<PurchaseState, GilDebited> next)
        where TException : Exception
    {
        logger.LogError(context.Exception,
            "[{CorrelationId}] GrantItemsActivity faulted — compensation may be needed",
            context.Saga.CorrelationId);

        await next.Faulted(context);
    }
}