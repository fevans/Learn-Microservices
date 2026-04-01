using Duende.IdentityServer;
using Duende.IdentityServer.Models;

namespace Identity.Service;

public static class IdentityServerConfig
{
    public static IEnumerable<IdentityResource> IdentityResources =>
    [
        new IdentityResources.OpenId(),
        new IdentityResources.Profile(),
        new IdentityResource("roles", "User roles", new[] { "role" })
    ];
    
    public static IEnumerable<ApiScope> ApiScopes =>
    [
        new ApiScope("catalog.fullaccess",   "Full access to Catalog API"),
        new ApiScope("inventory.fullaccess", "Full access to Inventory API"),
        new ApiScope("identity.fullaccess",  "Full access to Identity API"),
        new ApiScope("trading.fullaccess",   "Full access to Trading API"),
    ];
    
    public static IEnumerable<ApiResource> ApiResources =>
    [
        new ApiResource("catalog")
        {
            Scopes = { "catalog.fullaccess" },
            UserClaims = { "role" }
        },
        new ApiResource("inventory")
        {
            Scopes = { "inventory.fullaccess" },
            UserClaims = { "role" }
        },
        new ApiResource("identity")
        {
            Scopes = { "identity.fullaccess" },
            UserClaims = { "role" }
        },
    ];
    
    public static IEnumerable<Client> Clients (IConfiguration config ) =>
    [
        // Postman — for testing with client credentials (no user required)
        new Client
        {
            ClientId     = "postman-client",
            ClientName   = "Postman Client Credentials",
            AllowedGrantTypes = GrantTypes.ClientCredentials,
            ClientSecrets = { 
                new Secret(config["ClientSecrets:PostmanSecret"]!.Sha256()) },
            AllowedScopes =
            {
                "catalog.fullaccess",
                "inventory.fullaccess",
                "identity.fullaccess",
            }
        },

        // Postman — for testing with user credentials (requires registered user)
        new Client
        {
            ClientId     = "postman",
            ClientName   = "Postman",
            AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
            ClientSecrets =
            {
                new Secret(config["ClientSecrets:PostmanSecret"]!.Sha256())
            },
            AllowedScopes =
            {
                IdentityServerConstants.StandardScopes.OpenId,
                IdentityServerConstants.StandardScopes.Profile,
                "roles",
                "catalog.fullaccess",
                "inventory.fullaccess",
                "identity.fullaccess",
            },
            AlwaysIncludeUserClaimsInIdToken = true
        },

        // React SPA — Authorization Code + PKCE
        new Client
        {
            ClientId     = "play-frontend",
            ClientName   = "Play Economy Frontend",
            AllowedGrantTypes = GrantTypes.Code,
            RequirePkce           = true,
            RequireClientSecret   = false,
            RedirectUris          = { "http://localhost:3000/authentication/login-callback" },
            PostLogoutRedirectUris = { "http://localhost:3000/authentication/logout-callback" },
            AllowedCorsOrigins    = { "http://localhost:3000" },
            AllowedScopes =
            {
                IdentityServerConstants.StandardScopes.OpenId,
                IdentityServerConstants.StandardScopes.Profile,
                "roles",
                "catalog.fullaccess",
                "inventory.fullaccess",
            },
            AccessTokenLifetime     = 3600,  // 1 hour
            AllowOfflineAccess      = true,  // enables refresh tokens
            RefreshTokenUsage       = TokenUsage.OneTimeOnly,
            RefreshTokenExpiration  = TokenExpiration.Sliding,
            SlidingRefreshTokenLifetime = 86400,  // 24 hours
            AlwaysIncludeUserClaimsInIdToken = true,
        },
    ];
    

}