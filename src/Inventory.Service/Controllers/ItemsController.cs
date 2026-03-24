using GamePlatform.Common.Repositories;
using Inventory.Service.Dtos;
using Inventory.Service.Models;
using Microsoft.AspNetCore.Mvc;

namespace Inventory.Service.Controllers;

[ApiController]
[Route("items")]
public class ItemsController(IRepository<InventoryItem> inventoryItemsRepo,
    IRepository<CatalogItem> catalogItemsRepo) : ControllerBase
{
    /// <summary>
    /// Retrieves the inventory items for a specific user, including catalog details
    /// such as item name and description, along with the quantity and acquisition date.
    /// </summary>
    /// <param name="userId">
    /// The unique identifier of the user whose inventory items are to be retrieved.
    /// </param>
    /// <returns>
    /// An asynchronous operation that results in an HTTP response containing a collection of inventory items.
    /// Each item includes catalog information, quantity, and acquisition details.
    /// </returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<InventoryItemDto>>> GetAsync([FromQuery] Guid userId)
    {
        if (userId == Guid.Empty) return BadRequest("userId is required");

        var catalogItems = await catalogItemsRepo.GetAllAsync();
        var catalogMap = catalogItems.ToDictionary(i => i.Id);

        var inventoryItems = await inventoryItemsRepo.GetAllAsync();

        var result = inventoryItems
            .Where(i => i.UserId == userId)
            .Select(i =>
            {
                var cat = catalogMap.GetValueOrDefault(i.CatalogItemId);
                return new InventoryItemDto(
                    i.CatalogItemId,
                    cat?.Name ?? "Unknown",
                    cat?.Description,
                    i.Quantity,
                    i.AcquiredDate);
            });

        return Ok(result);
    }

    /// <summary>
    /// Processes a request to add or update inventory items for a user.
    /// If the inventory item exists, its quantity is updated.
    /// If it does not exist, a new inventory item is created.
    /// </summary>
    /// <param name="dto">
    /// A data transfer object containing the user ID, catalog item ID,
    /// and the quantity of the item to be granted.
    /// </param>
    /// <returns>
    /// An asynchronous operation that results in an HTTP response indicating
    /// the success or failure of the operation.
    /// </returns>
    [HttpPost]
    public async Task<ActionResult> PostAsync(GrantItemsDto dto)
    {
        var existing = (await inventoryItemsRepo.GetAllAsync())
            .FirstOrDefault(i => i.UserId == dto.UserId
                                 && i.CatalogItemId == dto.CatalogItemId);

        if (existing is null)
        {
            await inventoryItemsRepo.CreateAsync(new InventoryItem
            {
                Id = Guid.NewGuid(),
                UserId = dto.UserId,
                CatalogItemId = dto.CatalogItemId,
                Quantity = dto.Quantity,
                AcquiredDate = DateTimeOffset.UtcNow
            });
        }
        else
        {
            existing.Quantity += dto.Quantity;
            await inventoryItemsRepo.UpdateAsync(existing);
        }

        return Ok();
    }
}