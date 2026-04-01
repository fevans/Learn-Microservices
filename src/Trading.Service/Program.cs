using GamePlatform.Common.Settings;
using MassTransit;
using Trading.Service.StateMachines;
using Trading.Service.StateMachines.Activities;

var builder = WebApplication.CreateBuilder(args);
var mongoDbSettings = builder.Configuration.GetSection("MongoDbSettings").Get<MongoDbSettings>() ?? throw new InvalidOperationException("MongoDbSettings not configured");

builder.Services.AddMassTransit(x =>
{
    x.AddSagaStateMachine<PurchaseStateMachine, PurchaseState>()
        .MongoDbRepository(r =>
        {
            r.Connection = mongoDbSettings.ConnectionString;
            r.DatabaseName = mongoDbSettings.DatabaseName;
        });

    x.UsingRabbitMq((ctx, cfg) =>
    {
        var settings = builder.Configuration
            .GetSection(nameof(RabbitMqSettings))
            .Get<RabbitMqSettings>()
            ?? throw new InvalidOperationException("RabbitMQSettings not configured");

        cfg.Host(settings.Host, h =>
        {
            h.Username("guest");
            h.Password("guest");
        });

        cfg.ConfigureEndpoints(ctx);
    });
});

var app = builder.Build();
//
// // Configure the HTTP request pipeline.
// if (app.Environment.IsDevelopment())
// {
//     app.MapOpenApi();
// }

app.UseHttpsRedirection();
app.Run();
