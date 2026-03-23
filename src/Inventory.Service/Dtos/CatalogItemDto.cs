namespace Inventory.Service.Dtos;

public record CatalogItemDto
(
    Guid Id, 
    string Name, 
    string? Description
);