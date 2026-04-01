using GamePlatform.Identity.Contracts;
using GamePlatform.Inventory.Contracts;
using MassTransit;

namespace Trading.Service.StateMachines.Activities;

public class SubtractItemsActivity(ILogger<SubtractItemsActivity> logger)
    : IStateMachineActivity<PurchaseState, GilDebited>
{
    public void Accept(StateMachineVisitor visitor) => visitor.Visit(this);
    public void Probe(ProbeContext context) =>
        context.CreateScope(nameof(SubtractItemsActivity));

    public async Task Execute(
        BehaviorContext<PurchaseState, GilDebited> context,
        IBehavior<PurchaseState, GilDebited> next)
    {
        logger.LogWarning(
            "[{CorrelationId}] Compensating — subtracting {Quantity}x item {ItemId} from user {UserId}",
            context.Saga.CorrelationId,
            context.Saga.Quantity,
            context.Saga.ItemId,
            context.Saga.UserId);

        await context.Publish(new SubtractItems(
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
            "[{CorrelationId}] SubtractItemsActivity faulted — manual intervention may be required",
            context.Saga.CorrelationId);

        await next.Faulted(context);
    }
}