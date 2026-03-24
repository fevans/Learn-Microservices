using GamePlatform.Common;

namespace Inventory.Service.Models;

public class CatalogItem : IEntity
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
}