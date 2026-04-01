namespace GamePlatform.Identity.Contracts;

public record CreditGil(
    Guid    CorrelationId,
    Guid    UserId,
    decimal Gil);