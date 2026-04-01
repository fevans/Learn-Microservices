namespace Trading.Service.Contracts;

public record PurchaseRequested(
    Guid   PurchaseId,
    Guid   UserId,
    Guid   ItemId,
    int    Quantity);