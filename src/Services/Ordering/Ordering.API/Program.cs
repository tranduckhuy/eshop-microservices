using Ordering.Infrastructure;
using Ordering.Application;
using Microsoft.OpenApi.Models;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Ordering.Infrastructure.Data;
using MassTransit;
using Ordering.API.EventBusConsumer;
using EventBus.Messages.Common;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddApiVersioning();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHealthChecks().Services.AddDbContext<OrderContext>();
builder.Services.AddSwaggerGen(c => { c.SwaggerDoc("v1", new OpenApiInfo { Title = "Ordering.API", Version = "v1" }); });

builder.Services.AddMassTransit(config =>
{
    // Mark this as consumer
    config.AddConsumer<BasketOrderingConsumer>();
    config.UsingRabbitMq((ctx, cfg) =>
    {
        cfg.Host(builder.Configuration["EventBusSettings:HostAddress"]);
        // Provide the queue name with consumer settings
        cfg.ReceiveEndpoint(EventBusConstants.BasketCheckoutQueue, c =>
        {
            c.ConfigureConsumer<BasketOrderingConsumer>(ctx);
        });
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Ordering.API v1"));
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapHealthChecks("/health", new HealthCheckOptions()
{
    Predicate = _ => true,
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

// Seed data for the first time
await app.Services.SeedDataAsync();

await app.RunAsync();
