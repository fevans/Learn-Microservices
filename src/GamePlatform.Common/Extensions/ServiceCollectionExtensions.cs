using GamePlatform.Common.Entities;
using GamePlatform.Common.Repositories;
using GamePlatform.Common.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace GamePlatform.Common.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds MongoDB-related services to the service collection. This includes the configuration
    /// of MongoDB client, database, and repository for managing domain entities.
    /// </summary>
    /// <param name="services">The service collection to which the MongoDB services will be added.</param>
    /// <param name="configuration">The application configuration instance used to retrieve MongoDB settings.</param>
    /// <returns>The updated service collection with MongoDB services configured.</returns>
    public static IServiceCollection AddCatalogRepositories(this IServiceCollection services)
    {
        services.AddSingleton<IRepository<CatalogItem>>(sp =>
        {
            var mongoClient = sp.GetRequiredService<IMongoClient>();
            var database = mongoClient.GetDatabase("Catalog");
            return new MongoRepository<CatalogItem>(database, "items");
        });
        
        return services;
    }

    /// <summary>
    /// Configures the service collection to include MongoDB-related services. This setup includes
    /// initializing the MongoDB client with connection settings, enabling the application to
    /// interact with a MongoDB database.
    /// </summary>
    /// <param name="services">The service collection to which MongoDB services will be registered.</param>
    /// <param name="configuration">The configuration instance used to retrieve MongoDB connection settings.</param>
    /// <returns>The updated service collection with MongoDB services configured.</returns>
    public static IServiceCollection AddMongo(this IServiceCollection services, IConfiguration configuration)
    {
        var mongoDbSettings = configuration.GetSection(nameof(MongoDbSettings)).Get<MongoDbSettings>();
        
        services.AddSingleton<IMongoClient>(_ =>
            new MongoClient(mongoDbSettings!.ConnectionString));
        return services;
    }
}