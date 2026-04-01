namespace GamePlatform.Catalog.Contracts;

public record CatalogItemFound(
    Guid CorrelationId,
    Guid   CatalogItemId,
    string Name,
    decimal Price);