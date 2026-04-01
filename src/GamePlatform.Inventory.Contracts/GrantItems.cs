namespace GamePlatform.Inventory.Contracts;

public record GrantItems(
    Guid CorrelationId,
    Guid UserId,
    Guid CatalogItemId,
    int Quantity
);