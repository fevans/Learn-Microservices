using GamePlatform.Catalog.Contracts;
using GamePlatform.Common.Entities;
using GamePlatform.Common.Repositories;
using MassTransit;

namespace Catalog.Service.Consumers;

public class GetCatalogItemConsumer(IRepository<CatalogItem> repository)
    : IConsumer<GetCatalogItem>
{
    public async Task Consume(ConsumeContext<GetCatalogItem> context)
    {
        var item = await repository.GetAsync(context.Message.CatalogItemId);

        if (item is null)
        {
            await context.RespondAsync(
                new CatalogItemNotFound(context.Message.CatalogItemId));
            return;
        }

        await context.RespondAsync(
            new CatalogItemFound(item.Id, item.Name, item.Price));
    }
}