using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;


namespace Catalog.Service.Extensions;

public static class ConfigureServices
{
    /// <summary>
    /// Configures core service dependencies for the application, including controllers and API behavior options.
    /// This method ensures consistent API responses for invalid model states.
    /// </summary>
    /// <param name="services">The service collection to which the core service configurations will be added.</param>
    /// <returns>The updated service collection with core services configured.</returns>
    public static IServiceCollection AddServiceControllers(this IServiceCollection services)
    {
        services.AddControllers()
    
            .ConfigureApiBehaviorOptions(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                {
                    var errors = context.ModelState
                        .Where(e => e.Value?.Errors.Count > 0)
                        .ToDictionary(
                            e => e.Key,
                            e => e.Value!.Errors
                                .Select(x => x.ErrorMessage)
                                .ToArray()
                        );
                    return new BadRequestObjectResult(new
                    {
                        Title = "Validation failed",
                        Status = 400,
                        Errors = errors
                    });
                };
            });
        
        
        
        return services;
    }
    //
    // public static IServiceCollection AddMassTransmitService(this IServiceCollection services,
    //     IConfiguration configuration)
    // {
    //     services.AddMassTransit(x =>
    //     {
    //         x.UsingRabbitMq((ctx, cfg) =>
    //         {
    //             cfg.Host(configuration["RabbitMQSettings:Host"], h =>
    //             {
    //                 h.Username("guest");
    //                 h.Password("guest");
    //             });
    //
    //             cfg.ConfigureEndpoints(ctx);
    //         });
    //     });
    //     return services;
    // }
}