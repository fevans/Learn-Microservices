using GamePlatform.Common.Repositories;
using Inventory.Service.Models;
using MassTransit;
using GamePlatform.Catalog.Contracts;
using GamePlatform.Common;

namespace Inventory.Service.Consumers;

public class CatalogItemCreatedConsumer(IRepository<CatalogItem> repository) : IConsumer<CatalogItemCreated>
{
    public async Task Consume(ConsumeContext<CatalogItemCreated> context)
    {
        var message = context.Message;

        var item = await repository.GetAsync(message.ItemId);

        if (item is not null) return; // idempotency guard

        await repository.CreateAsync(new CatalogItem
        {
            Id = message.ItemId,
            Name = message.Name,
            Description = message.Description
        });
    }
}