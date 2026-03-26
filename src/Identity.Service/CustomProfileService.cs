using System.Security.Claims;
using Duende.IdentityServer.AspNetIdentity;
using Duende.IdentityServer.Models;
using Identity.Service.Models;
using Microsoft.AspNetCore.Identity;

namespace Identity.Service;

public class CustomProfileService(
    UserManager<ApplicationUser> userManager,
    IUserClaimsPrincipalFactory<ApplicationUser> claimsFactory)
    : ProfileService<ApplicationUser>(userManager, claimsFactory)
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;

    protected override async Task GetProfileDataAsync(ProfileDataRequestContext context, ApplicationUser user)
    {
        await base.GetProfileDataAsync(context, user);

        var roles = await _userManager.GetRolesAsync(user);
        
        var claims = new List<System.Security.Claims.Claim>();
        
        // Role claims
        claims.AddRange(roles.Select(r => new Claim("role", r)));
        
        // Custom domain claims
        claims.Add(new Claim("gil_spent", user.GilSpent.ToString()));

        // context.IssuedClaims.AddRange(roles.Select(role =>
        //     new System.Security.Claims.Claim("role", role)));
        //
        // // Also add role claims that were requested
        // var roleClaims = context.Subject.FindAll("role");
        context.IssuedClaims.AddRange(claims);
    }
}