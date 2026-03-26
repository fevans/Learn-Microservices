using Catalog.Service.Dtos;

using Catalog.Service.Extensions;
using GamePlatform.Catalog.Contracts;
using GamePlatform.Common.Entities;
using GamePlatform.Common.Identity;
using GamePlatform.Common.Repositories;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.Service.Controllers
{
    [Route("items")]
    [ApiController]
    public class ItemsController(IRepository<CatalogItem> repository, IPublishEndpoint publishEndpoint) : ControllerBase
    {
        //private readonly InMemoryRepository _repository;
        // GET: api/<ItemsController>
        [HttpGet]
        [Authorize(Policy = Policies.Read)]
        public async Task<ActionResult<IEnumerable<CatalogItemDto>>> GetAllAsync()
            => Ok ((await repository.GetAllAsync()).Select(item => item.AsDto()));
        
        // GET /items/{id}
        [Authorize(Policy = Policies.Read)]
        [HttpGet("{id:guid}", Name = nameof(GetByIdAsync))]
        public async Task<ActionResult<CatalogItemDto>> GetByIdAsync(Guid id)
        {
            var item = await repository.GetAsync(id);
            return item is null
                ? NotFound()
                : Ok(item.AsDto());
        }
        
        // POST /items
        [HttpPost]
        [Authorize(Policy = Policies.Write)]
        public async Task<ActionResult<CatalogItemDto>> CreateAsync(CreateItemDto dto)
        {
            if (dto.Price <= 0)
                return BadRequest(new { Error = "Price must be greater than zero." });
            
            var item = new CatalogItem

            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                CreatedDate = DateTimeOffset.UtcNow
            };
            await repository.CreateAsync(item);
            
            await publishEndpoint.Publish(new CatalogItemCreated(item.Id, item.Name, item.Description, item.Price));

            return CreatedAtRoute(nameof(GetByIdAsync),
                new { id = item .Id },
                item.AsDto());
        }
        
        // PUT /items/{id}
        [HttpPut("{id:guid}")]
        [Authorize(Policy = Policies.Write)]
        public async Task<IActionResult> Update(Guid id, UpdateItemDto dto)
        {
            if (dto.Price <= 0)
                return BadRequest(new { Error = "Price must be greater than zero." });
            
            var existing = await repository.GetAsync(id);
            if (existing is null) return NotFound();
            var updated = new CatalogItem
            {
                Id = id,
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                CreatedDate = existing.CreatedDate
            };
            await repository.UpdateAsync(updated);
            await publishEndpoint.Publish(new CatalogItemUpdated(
                updated.Id,
                updated.Name,
                updated.Description,
                updated.Price));
            return NoContent();
        }
        
        // DELETE /items/{id}
        [HttpDelete("{id:guid}")]
        [Authorize(Policy = Policies.Write)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var existing = await repository.GetAsync(id);
            if (existing is null) return NotFound();
            await repository.RemoveAsync(id);
            await publishEndpoint.Publish(new CatalogItemDeleted(id));
            return NoContent();
        }
        
        [HttpGet("premium")]
        [Authorize(Policy = "VeteranPlayer")]
        public IActionResult GetPremiumItems()
            => Ok(new[] { "Legendary Sword", "Dragon Armor" });
    }
}
