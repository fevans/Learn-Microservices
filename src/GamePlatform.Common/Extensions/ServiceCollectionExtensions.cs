using System.Reflection;
using GamePlatform.Common.Entities;
using GamePlatform.Common.Repositories;
using GamePlatform.Common.Settings;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace GamePlatform.Common.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMongoRepository<T>(this IServiceCollection services, string collectionName)
        where T : IEntity
    {
        services.AddSingleton<IRepository<T>>(sp =>
        {
            var db = sp.GetRequiredService<IMongoDatabase>();
            return new MongoRepository<T>(db, collectionName);
        });
        
        return services;
    }


    

    public static IServiceCollection AddMongo(this IServiceCollection services, IConfiguration config)
    {
        var settings = config
                           .GetSection(nameof(MongoDbSettings))
                           .Get<MongoDbSettings>()
                       ?? throw new InvalidOperationException("MongoDbSettings not configured");
        services.AddSingleton<IMongoClient>(_ => new MongoClient(settings.ConnectionString));
        services.AddSingleton(sp =>
            sp.GetRequiredService<IMongoClient>()
                .GetDatabase(settings.DatabaseName));
        return services;
    }
    
    public static IServiceCollection AddMassTransitWithRabbitMq(
        this IServiceCollection services,
        IConfiguration config,
        Action<IBusRegistrationConfigurator>? configure = null, bool registerConsumers = true)
    {
        services.AddMassTransit(x =>
        {
            // Auto-register all consumers in the calling assembly
            if (registerConsumers)
            {
                x.AddConsumers(Assembly.GetEntryAssembly());
                configure?.Invoke(x);
            }
            x.UsingRabbitMq((ctx, cfg) =>
            {
                var settings = config
                                   .GetSection(nameof(RabbitMqSettings))
                                   .Get<RabbitMqSettings>()
                               ?? throw new InvalidOperationException("RabbitMQSettings not configured");

                cfg.Host(settings.Host, h =>
                {
                    h.Username("guest");
                    h.Password("guest");
                });

                cfg.ConfigureEndpoints(ctx);
            });
        });

        return services;
    }
}