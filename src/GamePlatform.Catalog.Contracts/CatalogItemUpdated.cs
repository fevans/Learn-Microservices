namespace GamePlatform.Catalog.Contracts;

public record CatalogItemUpdated(Guid ItemId, 
    string Name, 
    string? Description, 
    decimal Price);