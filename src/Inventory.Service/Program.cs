using GamePlatform.Common.Extensions;
using Inventory.Service.Extensions;
using Inventory.Service.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers( o => o.SuppressAsyncSuffixInActionNames = false );
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddMongo(builder.Configuration)
    .AddMongoRepository<InventoryItem>(collectionName: "inventoryitems");
builder.Services.AddHttpClientAndResiliencePolicy(builder.Configuration);


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseAuthorization();
app.MapControllers();
app.Run();
