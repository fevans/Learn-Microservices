using Catalog.Service;
using Catalog.Service.Extensions;
using GamePlatform.Common.Entities;
using GamePlatform.Common.Extensions;
using GamePlatform.Common.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

using MongoDB.Driver;

BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));

var builder = WebApplication.CreateBuilder(args);
// Catalog.Service/Program.cs
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
app.UseAuthentication();   // ← who are you?
app.UseAuthorization();    // ← what can you do?
app.MapControllers();
app.Run();
