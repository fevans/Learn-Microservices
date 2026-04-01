namespace Trading.Service.Contracts;

public record PurchaseFailed(
    Guid   PurchaseId,
    string Reason);