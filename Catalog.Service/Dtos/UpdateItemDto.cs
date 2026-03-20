using System.ComponentModel.DataAnnotations;

namespace Catalog.Service.Dtos;

public record UpdateItemDto(
    [Required]
    [StringLength(100, MinimumLength = 1)]
    string Name,
    [Required]
    [StringLength(500)]
    string Description,
    [Range(0, 1_000_000)]
    decimal Price
);