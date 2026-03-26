using System.Security.Cryptography;
using GamePlatform.Common.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GamePlatform.Common.Identity;

public static class Extensions
{
    public static IServiceCollection AddGamePlatformAuthentication(this IServiceCollection services, IConfiguration config)
    {
        var serviceSettings = config
                                  .GetSection(nameof(ServiceSettings))
                                  .Get<ServiceSettings>()
                              ?? throw new InvalidOperationException("ServiceSettings not configured");

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Authority = serviceSettings.Authority;
                options.Audience = serviceSettings.ServiceName;
                options.MapInboundClaims = false;
                options.TokenValidationParameters.RoleClaimType = "role";
            });

        services.AddAuthorization(options =>
        {
            // standard role-based policies
            options.AddPolicy(Policies.Read, policy => { policy.RequireRole("Admin", "Player"); });
            options.AddPolicy(Policies.Write, policy => { policy.RequireRole("Admin"); });
            
            // Custom claims policy — user must have spent at least 100 gil
            options.AddPolicy("VeteranPlayer", policy =>
                policy.RequireAssertion(ctx =>
                {
                    var gilSpentClaim = ctx.User.FindFirst("gil_spent");
                    return gilSpentClaim is not null
                           && decimal.TryParse(gilSpentClaim.Value, out var gilSpent)
                           && gilSpent >= 100;
                }));
        });

        return services;
    }
}