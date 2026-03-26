using Microsoft.AspNetCore.Authorization;

namespace Catalog.Service.Authorization;

public class MinimumGilRequirement (decimal minimumGil): IAuthorizationRequirement
{
    public decimal MinimumGil { get; } = minimumGil;
}

public class MinimumGilHandler : AuthorizationHandler<MinimumGilRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, 
        MinimumGilRequirement requirement)
    {
        var gilClaim = context.User.Claims.FirstOrDefault(c => c.Type == "gil_spent");
        
        if (gilClaim is not null 
            && decimal.TryParse(gilClaim.Value, out var gilSpent)
            && gilSpent >= requirement.MinimumGil)
        {
                context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}