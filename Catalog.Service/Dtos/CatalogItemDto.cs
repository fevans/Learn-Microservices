namespace Catalog.Service.Dtos;

// Returned on Get Requests
public record CatalogItemDto( 
    Guid Id, 
    string Name, 
    string Description, 
    decimal Price, 
    DateTimeOffset CreatedDate
    );