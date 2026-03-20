using Catalog.Service.Dtos;
using GamePlatform.Common.Entities;

namespace Catalog.Service.Extensions;

public static class DtoExtensions
{
    public static CatalogItemDto AsDto(this CatalogItem item) => 
        new(item.Id,
            item.Name,
            item.Description,
            item.Price,
            item.CreatedDate
        );

}