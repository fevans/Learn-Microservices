using MassTransit;

namespace Trading.Service.StateMachines;

public class PurchaseState : SagaStateMachineInstance, ISagaVersion
{
    public Guid   CorrelationId  { get; set; }
    public int    Version        { get; set; }
    public string CurrentState   { get; set; } = null!;

    // Participants
    public Guid   UserId         { get; set; }
    public Guid   ItemId         { get; set; }
    public int    Quantity       { get; set; }

    // Calculated during saga
    public string? ItemName      { get; set; }
    public decimal? PurchaseTotal { get; set; }

    // Timing
    public DateTimeOffset Received    { get; set; }
    public DateTimeOffset? LastUpdated { get; set; }

    // Error tracking
    public string? ErrorMessage  { get; set; }
}