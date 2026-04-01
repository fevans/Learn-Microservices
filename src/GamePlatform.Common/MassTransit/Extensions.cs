using System.Reflection;
using GamePlatform.Common.Settings;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GamePlatform.Common.MassTransit;

public static class Extensions
{
    private static readonly TimeSpan DefaultRetryInterval = TimeSpan.FromSeconds(5);

    public static IServiceCollection AddMassTransitWithRabbitMq(
        this IServiceCollection services,
        IConfiguration config,
        Action<IRetryConfigurator>? configureRetries = null)
    {
        services.AddMassTransit(x =>
        {
            x.AddConsumers(Assembly.GetEntryAssembly());

            // Allow state machine sagas to be registered by the calling service
            x.AddSagaStateMachines(Assembly.GetEntryAssembly());
            x.AddSagas(Assembly.GetEntryAssembly());
            x.AddActivities(Assembly.GetEntryAssembly());

            x.UsingRabbitMq((ctx, cfg) =>
            {
                var settings = config
                                   .GetSection(nameof(RabbitMqSettings))
                                   .Get<RabbitMqSettings>()
                               ?? throw new InvalidOperationException(
                                   "RabbitMQSettings not configured");

                cfg.Host(settings.Host, h =>
                {
                    h.Username("guest");
                    h.Password("guest");
                });

                // Apply retry policy — custom or default
                cfg.UseMessageRetry(retryConfig =>
                {
                    if (configureRetries is not null)
                        configureRetries(retryConfig);
                    else
                        retryConfig.Interval(3, DefaultRetryInterval);
                });

                cfg.UseInMemoryOutbox(ctx);

                cfg.ConfigureEndpoints(ctx);
            });
        });

        return services;
    }   
}