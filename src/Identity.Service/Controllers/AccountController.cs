using Duende.IdentityServer.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Identity.Service.Models;
using System.Security.Claims;
using System.Text.Json.Serialization;


namespace Identity.Service.Controllers;

[Route("[controller]")]
public class AccountController (SignInManager<ApplicationUser> signInManager,
    UserManager<ApplicationUser> userManager,
    IIdentityServerInteractionService interaction): Controller
{
    [HttpGet("login")]
    [AllowAnonymous]
    public IActionResult Login(string returnUrl, string? login_hint)
    {
        // Return a simple HTML login form
        var html = $@"
<!DOCTYPE html>
<html>
<head>
    <title>Login</title>
    <style>
        body {{ font-family: sans-serif; max-width: 400px; margin: 50px auto; padding: 20px; }}
        input {{ width: 100%; padding: 8px; margin: 8px 0; box-sizing: border-box; }}
        button {{ width: 100%; padding: 10px; background: #007bff; color: white; border: none; cursor: pointer; }}
        button:hover {{ background: #0056b3; }}
        .error {{ color: red; }}
    </style>
</head>
<body>
    <h2>Sign In</h2>
    <form method='post' action='/Account/login'>
        <input type='hidden' name='returnUrl' value='{returnUrl}' />
        <label>Email</label>
        <input type='email' name='email' value='{login_hint}' required />
        <label>Password</label>
        <input type='password' name='password' required />
        <button type='submit'>Sign In</button>
    </form>
</body>
</html>";
        return Content(html, "text/html");
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> LoginPost(string email, string password, string returnUrl)
    {
        var user = await userManager.FindByEmailAsync(email);

        if (user == null)
        {
            return BadRequest(new { message = "Invalid email or password" });
        }

        var result = await signInManager.PasswordSignInAsync(user, password, isPersistent: false, lockoutOnFailure: false);

        if (!result.Succeeded)
        {
            return BadRequest(new { message = "Invalid email or password" });
        }

        if (!string.IsNullOrEmpty(returnUrl))
        {
            return Redirect(returnUrl);
        }

        return Redirect("/");
    }

    [HttpPost("register")]
    [AllowAnonymous]
    [Consumes("application/json")]
    public async Task<IActionResult> Register([FromBody] RegisterDto? dto)
    {
        if (dto == null)
        {
            return BadRequest(new { message = "DTO is null" });
        }

        if (string.IsNullOrEmpty(dto.Username))
        {
            return BadRequest(new { message = $"Username is null or empty. DTO: {System.Text.Json.JsonSerializer.Serialize(dto)}" });
        }

        if (string.IsNullOrEmpty(dto.Email) || string.IsNullOrEmpty(dto.Password))
        {
            return BadRequest(new { message = $"Email or Password missing. DTO: {System.Text.Json.JsonSerializer.Serialize(dto)}" });
        }

        var user = new ApplicationUser
        {
            UserName = dto.Username,
            Email = dto.Email,
            Gil = dto.StartingGil
        };

        var result = await userManager.CreateAsync(user, dto.Password);

        if (!result.Succeeded)
        {
            return BadRequest(new { message = string.Join(", ", result.Errors.Select(e => e.Description)) });
        }

        // Try to assign Player role, but don't fail if role doesn't exist
        try
        {
            var roleExists = await userManager.GetRolesAsync(user);
            if (roleExists != null)
            {
                await userManager.AddToRoleAsync(user, "Player");
            }
        }
        catch
        {
            // Role assignment failed, but user is created
        }

        return Ok(new { message = "User created successfully" });
    }

    [HttpGet("logout")]
    public async Task<IActionResult> Logout(string logoutId)
    {
        var context = await interaction.GetLogoutContextAsync(logoutId);

        await signInManager.SignOutAsync();

        await HttpContext.SignOutAsync();

        var postLogoutRedirectUri = context?.PostLogoutRedirectUri
                                    ?? "http://localhost:3000";

        return Redirect(postLogoutRedirectUri);
    }
}

public class RegisterDto
{
    [JsonPropertyName("username")]
    public string? Username { get; set; }

    [JsonPropertyName("email")]
    public string? Email { get; set; }

    [JsonPropertyName("password")]
    public string? Password { get; set; }

    [JsonPropertyName("startingGil")]
    public int StartingGil { get; set; }
}