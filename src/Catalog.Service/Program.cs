using Catalog.Service;
using Catalog.Service.Extensions;
using GamePlatform.Common.Extensions;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;

BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));

var builder = WebApplication.CreateBuilder(args);
builder.Services
    .AddServiceControllers()
    .AddMongo(builder.Configuration)
    .AddCatalogRepositories()
    .AddEndpointsApiExplorer()
    .AddSwaggerGen();


var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger(_ => { });
    app.UseSwaggerUI();
}

// configure http request pipeline
app.UseAuthorization();
app.MapControllers();
app.Run();
