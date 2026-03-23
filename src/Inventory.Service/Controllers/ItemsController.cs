using GamePlatform.Common.Repositories;
using Inventory.Service.Clients;
using Inventory.Service.Dtos;
using Inventory.Service.Models;
using Microsoft.AspNetCore.Mvc;

namespace Inventory.Service.Controllers;

[ApiController]
[Route("items")]
public class ItemsController(IRepository<InventoryItem> repository, CatalogClient client) : ControllerBase
{
    public async Task<ActionResult<IEnumerable<InventoryItemDto>>> GetAsync(
        [FromQuery] Guid userId)
    {
        if (userId == Guid.Empty) return BadRequest("userId is required");

        var catalogItems = await client.GetCatalogItemsAsync();
        var catalogMap = catalogItems.ToDictionary(i => i.Id);

        var inventoryItems = await repository.GetAllAsync();

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
    
    [HttpPost]
    public async Task<ActionResult> PostAsync(GrantItemsDto dto)
    {
        var existing = (await repository.GetAllAsync())
            .FirstOrDefault(i => i.UserId == dto.UserId
                                 && i.CatalogItemId == dto.CatalogItemId);

        if (existing is null)
        {
            await repository.CreateAsync(new InventoryItem
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
            await repository.UpdateAsync(existing);
        }

        return Ok();
    }
}