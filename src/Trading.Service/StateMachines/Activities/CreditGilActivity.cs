using GamePlatform.Identity.Contracts;
using GamePlatform.Inventory.Contracts;
using MassTransit;

namespace Trading.Service.StateMachines.Activities;

public class CreditGilActivity(ILogger<CreditGilActivity> logger)
    : IStateMachineActivity<PurchaseState, InventoryItemsSubtracted>
{
    public void Accept(StateMachineVisitor visitor) => visitor.Visit(this);
    public void Probe(ProbeContext context) =>
        context.CreateScope(nameof(CreditGilActivity));

    public async Task Execute(
        BehaviorContext<PurchaseState, InventoryItemsSubtracted> context,
        IBehavior<PurchaseState, InventoryItemsSubtracted> next)
    {
        logger.LogWarning(
            "[{CorrelationId}] Compensating — crediting {Total} gil back to user {UserId}",
            context.Saga.CorrelationId,
            context.Saga.PurchaseTotal,
            context.Saga.UserId);

        await context.Publish(new CreditGil(
            CorrelationId: context.Saga.CorrelationId,
            UserId:        context.Saga.UserId,
            Gil:           context.Saga.PurchaseTotal!.Value));

        await next.Execute(context);
    }

    public async Task Faulted<TException>(
        BehaviorExceptionContext<PurchaseState, InventoryItemsSubtracted, TException> context,
        IBehavior<PurchaseState, InventoryItemsSubtracted> next)
        where TException : Exception
    {
        logger.LogError(context.Exception,
            "[{CorrelationId}] CreditGilActivity faulted — manual intervention may be required",
            context.Saga.CorrelationId);

        await next.Faulted(context);
    }
}