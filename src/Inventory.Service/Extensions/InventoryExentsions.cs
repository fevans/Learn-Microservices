using Inventory.Service.Clients;
using Inventory.Service.Policies;

namespace Inventory.Service.Extensions;

public static class InventoryExentsions
{
    public static IServiceCollection AddHttpClientAndResiliencePolicy(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddHttpClient<CatalogClient>(client =>
        {
            client.BaseAddress = new Uri(configuration["CatalogServiceUrl"]
                                         ?? throw new InvalidOperationException("CatalogServiceUrl not configured"));
        })
        .AddPolicyHandler(PollyPolicies.GetRetryPolicy())
        .AddPolicyHandler(PollyPolicies.GetCircuitBreakerPolicy())
        .AddPolicyHandler(PollyPolicies.GetTimeoutPolicy());
        
        return services;
    }


}