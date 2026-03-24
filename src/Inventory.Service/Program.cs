using GamePlatform.Common.Extensions;
using Inventory.Service.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services
    .AddControllers(o => o.SuppressAsyncSuffixInActionNames = false);
    builder.Services.AddCors(options =>
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
    .AddMongoRepository<CatalogItem>(collectionName: "catalogitems");
//builder.Services.AddHttpClientAndResiliencePolicy(builder.Configuration);
builder.Services.AddMassTransitWithRabbitMq(builder.Configuration, registerConsumers: true);


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors();
app.UseAuthorization();
app.MapControllers();
app.Run();
