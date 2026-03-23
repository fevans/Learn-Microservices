using Inventory.Service.Dtos;
using Inventory.Service.Models;

namespace Inventory.Service.Clients;

public class CatalogClient(HttpClient httpClient)
{
    public async Task<IReadOnlyCollection<CatalogItemDto>> GetCatalogItemsAsync()
    {
        var items = await httpClient
            .GetFromJsonAsync<IReadOnlyCollection<CatalogItemDto>>("/items");
        
        return items ?? [];
    }
}