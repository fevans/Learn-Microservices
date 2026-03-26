using AspNetCore.Identity.MongoDbCore.Extensions;
using AspNetCore.Identity.MongoDbCore.Infrastructure;
using GamePlatform.Common.Identity;
using Identity.Service;
using Identity.Service.Extensions;
using Identity.Service.Models;
using Microsoft.AspNetCore.Identity;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using Game = GamePlatform.Common.Settings;

var builder = WebApplication.CreateBuilder(args);
BsonSerializer.RegisterSerializer(new GuidSerializer(MongoDB.Bson.BsonType.String));
BsonSerializer.RegisterSerializer(new DateTimeOffsetSerializer(MongoDB.Bson.BsonType.String));

var mongoDbSettings = builder.Configuration
    .GetSection(nameof(MongoDbSettings))
    .Get<Game.MongoDbSettings>()!;

// MongoDB Identity configuration
var mongoDbIdentityConfig = new MongoDbIdentityConfiguration
{
    MongoDbSettings = new MongoDbSettings
    {
        ConnectionString = mongoDbSettings.ConnectionString,
        DatabaseName = mongoDbSettings.DatabaseName
    },
    IdentityOptionsAction = options =>
    {
        options.Password.RequiredLength = 8;
        options.Password.RequireDigit = false;
        options.Password.RequireUppercase = false;
        options.Password.RequireNonAlphanumeric = false;

        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
        options.Lockout.MaxFailedAccessAttempts = 5;

        options.User.RequireUniqueEmail = true;
    }
};
// Add Data Protection services (required by Identity token providers)
builder.Services.AddDataProtection();

builder.Services.ConfigureMongoDbIdentity<ApplicationUser, ApplicationRole, Guid>(mongoDbIdentityConfig)
    .AddUserManager<UserManager<ApplicationUser>>()
    .AddSignInManager<SignInManager<ApplicationUser>>()
    .AddRoleManager<RoleManager<ApplicationRole>>()
    .AddDefaultTokenProviders();

builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = "Cookies";
        options.DefaultChallengeScheme = "Cookies";
    })
    .AddCookie("Cookies");
builder.Services.AddGamePlatformAuthentication(builder.Configuration);


// After Identity registration, before builder.Build()
builder.Services
    .AddIdentityServer(options =>
    {
        options.Authentication.CookieAuthenticationScheme = "Cookies";
        options.Events.RaiseErrorEvents       = true;
        options.Events.RaiseInformationEvents = true;
        options.Events.RaiseFailureEvents     = true;
        options.Events.RaiseSuccessEvents     = true;
    })
    .AddAspNetIdentity<ApplicationUser>()
    .AddProfileService<CustomProfileService>()
    .AddInMemoryIdentityResources(IdentityServerConfig.IdentityResources)
    .AddInMemoryApiScopes(IdentityServerConfig.ApiScopes)
    .AddInMemoryApiResources(IdentityServerConfig.ApiResources)
    .AddInMemoryClients(IdentityServerConfig.Clients(builder.Configuration))
    .AddDeveloperSigningCredential(); // ← dev only; replaced with real cert in Section 29

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy
            .WithOrigins(builder.Configuration["AllowedOrigins"]!)
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
//Seed  roles - after app.Build
await app.SeedRolesAsync();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();
app.UseIdentityServer();   // ← must remain before UseAuthentication
app.UseAuthentication();   // ← who are you?
app.UseAuthorization();
app.MapControllers();
app.Run();