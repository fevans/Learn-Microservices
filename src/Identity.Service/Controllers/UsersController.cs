using Identity.Service.Dtos;
using Identity.Service.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Service.Controllers;

[ApiController]
[Route("users")]
public class UsersController (UserManager<ApplicationUser> userManager): ControllerBase
{
    // GET /users
    [HttpGet]
    [Authorize]
    public ActionResult<IEnumerable<UserDto>> Get()
    {
        var users = userManager.Users.ToList().Select(u => u.AsDto());
        return Ok(users);
    }
    
    
    // GET /users/{id}
    [Authorize]
    [HttpGet("{id}", Name = nameof(GetByIdAsync))]
    public async Task<ActionResult<UserDto>> GetByIdAsync(Guid id)
    {
        var user = await userManager.FindByIdAsync(id.ToString());
        return user is null ? NotFound() : user.AsDto();
    }
    
    // POST /users
    [HttpPost]
    [AllowAnonymous]
    public async Task<ActionResult<UserDto>> RegisterAsync(RegisterUserDto dto)
    {
        var existingUser = await userManager.FindByEmailAsync(dto.Email);
        if (existingUser is not null)
        {
            return Conflict(new { message = $"Email '{dto.Email}' is already registered." });
        }

        var user = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            UserName = dto.Username,
            Email = dto.Email,
            Gil = dto.StartingGil
          //  CreatedOn = DateTime.UtcNow
        };

        var result = await userManager.CreateAsync(user, dto.Password);

        if (!result.Succeeded)
        {
            return BadRequest(result.Errors);
        }

        return CreatedAtRoute(nameof(GetByIdAsync),
            new { id = user.Id },
            user.AsDto());
    }
    
    // DELETE /users/{id}
    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAsync(Guid id)
    {
        var user = await userManager.FindByIdAsync(id.ToString());
        if (user is null) return NotFound();

        await userManager.DeleteAsync(user);
        return NoContent();
    }
}