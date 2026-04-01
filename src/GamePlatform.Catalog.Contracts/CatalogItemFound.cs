namespace GamePlatform.Catalog.Contracts;

public record CatalogItemFound(
    Guid   CatalogItemId,
    string Name,
    decimal Price);