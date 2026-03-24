using GamePlatform.Catalog.Contracts;
using GamePlatform.Common.Repositories;
using Inventory.Service.Models;
using MassTransit;

namespace Inventory.Service.Consumers;

public class CatalogItemUpdatedConsumer(IRepository<CatalogItem> repository)
    : IConsumer<CatalogItemUpdated>
{
    public async Task Consume(ConsumeContext<CatalogItemUpdated> context)
    {
        var message = context.Message;

        var item = await repository.GetAsync(message.ItemId);

        if (item is null)
        {
            await repository.CreateAsync(new CatalogItem
            {
                Id = message.ItemId,
                Name = message.Name,
                Description = message.Description
            });
        }
        else
        {
            item.Name = message.Name;
            item.Description = message.Description;
            await repository.UpdateAsync(item);
        }
    }
}