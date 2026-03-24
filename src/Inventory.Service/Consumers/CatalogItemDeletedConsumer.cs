using GamePlatform.Catalog.Contracts;
using GamePlatform.Common.Repositories;
using Inventory.Service.Models;
using MassTransit;

namespace Inventory.Service.Consumers;

public class CatalogItemDeletedConsumer(IRepository<CatalogItem> repository)
    : IConsumer<CatalogItemDeleted>
{
    public async Task Consume(ConsumeContext<CatalogItemDeleted> context)
    {
        var item = await repository.GetAsync(context.Message.ItemId);
        if (item is null) return;
        await repository.RemoveAsync(context.Message.ItemId);
    }
}