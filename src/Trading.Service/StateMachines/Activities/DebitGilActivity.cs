using GamePlatform.Catalog.Contracts;
using GamePlatform.Identity.Contracts;
using MassTransit;

namespace Trading.Service.StateMachines.Activities;

public class DebitGilActivity(ILogger<DebitGilActivity> logger)
    : IStateMachineActivity<PurchaseState, CatalogItemFound>
{
    public void Accept(StateMachineVisitor visitor) => visitor.Visit(this);
    public void Probe(ProbeContext context) =>
        context.CreateScope(nameof(DebitGilActivity));

    public async Task Execute(
        BehaviorContext<PurchaseState, CatalogItemFound> context,
        IBehavior<PurchaseState, CatalogItemFound> next)
    {
        logger.LogInformation(
            "[{CorrelationId}] Debiting {Total} gil from user {UserId}",
            context.Saga.CorrelationId,
            context.Saga.PurchaseTotal,
            context.Saga.UserId);

        // PurchaseTotal is set by the state machine's Then() block
        // before this activity runs, so it is guaranteed non-null here
        await context.Publish(new DebitGil(
            CorrelationId: context.Saga.CorrelationId,
            UserId:        context.Saga.UserId,
            Gil:           context.Saga.PurchaseTotal!.Value));

        await next.Execute(context);
    }

    public async Task Faulted<TException>(
        BehaviorExceptionContext<PurchaseState, CatalogItemFound, TException> context,
        IBehavior<PurchaseState, CatalogItemFound> next)
        where TException : Exception
    {
        logger.LogError(context.Exception,
            "[{CorrelationId}] DebitGilActivity faulted",
            context.Saga.CorrelationId);

        await next.Faulted(context);
    }
}