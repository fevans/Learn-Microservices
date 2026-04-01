using GamePlatform.Common;
using GamePlatform.Inventory.Contracts;
using Inventory.Service.Models;
using MassTransit;
using System.Linq;
using GamePlatform.Common.Repositories;
using Inventory.Service.Exceptions;

namespace Inventory.Service.Consumers;

public class GrantItemsConsumer(
    IRepository<InventoryItem> inventoryRepository,
    IRepository<CatalogItem> catalogRepository)
    : IConsumer<GrantItems>
{
    public async Task Consume(ConsumeContext<GrantItems> context)
    {
        var message = context.Message;
        // verify that the catalog item exists in the local projection?
        var catalogItem = await catalogRepository.GetAsync(message.CatalogItemId);
        if  (catalogItem is null)
            throw new UnknownItemException (message.CatalogItemId);


        var inventoryItem = (await inventoryRepository.GetAllAsync())
            .FirstOrDefault(i => i.UserId == message.UserId && i.CatalogItemId == message.CatalogItemId);
        
        if (inventoryItem is null)
        {
            await inventoryRepository.CreateAsync(new InventoryItem
            {
                Id            = Guid.NewGuid(),
                UserId        = message.UserId,
                CatalogItemId = message.CatalogItemId,
                Quantity      = message.Quantity,
                AcquiredDate  = DateTimeOffset.UtcNow
            });
        }
        else
        {
            inventoryItem.Quantity += message.Quantity;
            await inventoryRepository.UpdateAsync(inventoryItem);
        }

        await context.Publish(new InventoryItemsGranted(message.CorrelationId));

    }
}

public class SubtractItemsConsumer(
    IRepository<InventoryItem> inventoryRepository)
    : IConsumer<SubtractItems>
{
    public async Task Consume(ConsumeContext<SubtractItems> context)
    {
        var message = context.Message;
        

        var inventoryItem = (await inventoryRepository.GetAllAsync())
            .FirstOrDefault(i => i.UserId         == message.UserId
                                 && i.CatalogItemId   == message.CatalogItemId);

        if (inventoryItem is not null)
        {
            inventoryItem.Quantity -= message.Quantity;

            if (inventoryItem.Quantity <= 0)
                await inventoryRepository.RemoveAsync(inventoryItem.Id);
            else
                await inventoryRepository.UpdateAsync(inventoryItem);
        }

        await context.Publish(
            new InventoryItemsSubtracted(message.CorrelationId));
    }
}