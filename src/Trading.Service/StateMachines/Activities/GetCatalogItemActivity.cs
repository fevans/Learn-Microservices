using GamePlatform.Catalog.Contracts;
using MassTransit;
using Trading.Service.Contracts;

namespace Trading.Service.StateMachines.Activities;

public class GetCatalogItemActivity(IRequestClient<GetCatalogItem> client, ILogger<GetCatalogItemActivity> logger)
    : IStateMachineActivity<PurchaseState, PurchaseRequested>
{
    public void Accept(StateMachineVisitor visitor) => visitor.Visit(this);
    public void Probe(ProbeContext context) =>
        context.CreateScope(nameof(GetCatalogItemActivity));

    public async Task Execute(
        BehaviorContext<PurchaseState, PurchaseRequested> context,
        IBehavior<PurchaseState, PurchaseRequested> next)
    {
        logger.LogInformation(
            "[{CorrelationId}] Requesting catalog item price for item {ItemId}",
            context.Saga.CorrelationId,
            context.Saga.ItemId);

        // Use MassTransit request/response — waits for one of the two response types
        var (found, notFound) = await client
            .GetResponse<CatalogItemFound, CatalogItemNotFound>(
                new GetCatalogItem(context.Saga.ItemId));

        if (found.IsCompletedSuccessfully)
        {
            var response = await found;

            logger.LogInformation(
                "[{CorrelationId}] Catalog item found: {Name} @ {Price}",
                context.Saga.CorrelationId,
                response.Message.Name,
                response.Message.Price);

            // Re-publish as a correlated event so the state machine
            // can process it in the During(Accepted) block
            await context.Publish(new CatalogItemFound(
                CorrelationId: context.Saga.CorrelationId,
                CatalogItemId: response.Message.CatalogItemId,
                Name:          response.Message.Name,
                Price:         response.Message.Price));
        }
        else
        {
            var response = await notFound;

            logger.LogWarning(
                "[{CorrelationId}] Catalog item not found: {ItemId}",
                context.Saga.CorrelationId,
                response.Message.CatalogItemId);

            await context.Publish(new CatalogItemNotFound(
                CorrelationId: context.Saga.CorrelationId,
                CatalogItemId: response.Message.CatalogItemId));
        }

        await next.Execute(context);
    }

    public async Task Faulted<TException>(
        BehaviorExceptionContext<PurchaseState, PurchaseRequested, TException> context,
        IBehavior<PurchaseState, PurchaseRequested> next)
        where TException : Exception
    {
        logger.LogError(context.Exception,
            "[{CorrelationId}] GetCatalogItemActivity faulted",
            context.Saga.CorrelationId);

        await next.Faulted(context);
    }
}