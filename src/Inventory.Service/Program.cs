using GamePlatform.Common.Extensions;
using GamePlatform.Common.Identity;
using Inventory.Service.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services
    .AddControllers(o => o.SuppressAsyncSuffixInActionNames = false);
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme);
builder.Services
    .AddGamePlatformAuthentication(builder.Configuration)
    .AddAuthorization()
    .AddCors(options =>
    {
        options.AddDefaultPolicy(policy =>
        {
            policy
                .WithOrigins(builder.Configuration["AllowedOrigins"]!)
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
    })
    .AddOpenApi()
    .AddMongo(builder.Configuration)
    .AddMongoRepository<InventoryItem>(collectionName: "inventoryitems")
    .AddMongoRepository<CatalogItem>(collectionName: "catalogitems") 
    .AddMassTransitWithRabbitMq(builder.Configuration, registerConsumers: true);


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
