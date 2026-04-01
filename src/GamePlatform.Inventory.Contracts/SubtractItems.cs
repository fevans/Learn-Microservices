namespace GamePlatform.Inventory.Contracts;

public record SubtractItems(
    Guid   CorrelationId,
    Guid   UserId,
    Guid   CatalogItemId,
    int    Quantity);