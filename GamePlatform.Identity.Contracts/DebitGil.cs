namespace GamePlatform.Identity.Contracts;

public record DebitGil(
    Guid    CorrelationId,
    Guid    UserId,
    decimal Gil);