using Catalog.Service;
using Catalog.Service.Extensions;
using GamePlatform.Common.Entities;
using GamePlatform.Common.Extensions;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

using MongoDB.Driver;

BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));

var builder = WebApplication.CreateBuilder(args);
builder.Services
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
    .AddServiceControllers()
    .AddMongo(builder.Configuration)
    .AddMongoRepository<CatalogItem>(collectionName: "items")
    //.AddMassTransmitService(builder.Configuration)
    .AddMassTransitWithRabbitMq(builder.Configuration)
    .AddEndpointsApiExplorer()
    .AddSwaggerGen();


var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger(_ => { });
    app.UseSwaggerUI();
}

// configure http request pipeline
app.UseRouting();
app.UseCors();
app.UseAuthorization();
app.MapControllers();
app.Run();
