using GamePlatform.Common.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GamePlatform.Common.Identity;

public static class Extensions
{
    public static IServiceCollection AddGamePlatformAuthentication(
        this IServiceCollection services,
        IConfiguration config)
    {
        var serviceSettings = config
                                  .GetSection(nameof(ServiceSettings))
                                  .Get<ServiceSettings>()
                              ?? throw new InvalidOperationException("ServiceSettings not configured");

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Authority        = serviceSettings.Authority;
                options.Audience         = serviceSettings.ServiceName;
                options.MapInboundClaims = false;

                // For development only: allow HTTP authority and bypass SSL validation
                if (serviceSettings.Authority?.StartsWith("http://", StringComparison.OrdinalIgnoreCase) == true)
                {
                    options.RequireHttpsMetadata = false;
                    options.BackchannelHttpHandler = new HttpClientHandler
                    {
                        ServerCertificateCustomValidationCallback =
                            HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                    };
                }
            });

        return services;
    }
}