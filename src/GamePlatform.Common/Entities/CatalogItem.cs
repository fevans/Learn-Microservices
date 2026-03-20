namespace GamePlatform.Common.Entities;

public class CatalogItem : IEntity
{
    public Guid Id { get; set; }
    public required string Name { get; init; }
    public required string Description { get; init; }
    public decimal Price { get; init; }
    public DateTimeOffset CreatedDate { get; init; }
}