namespace GamePlatform.Catalog.Contracts;

public record CatalogItemNotFound(  Guid CorrelationId, Guid CatalogItemId);