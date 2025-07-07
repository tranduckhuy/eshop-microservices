using Asp.Versioning;
using Asp.Versioning.Conventions;
using Common.Logging;
using Common.Logging.Correlation;
using EventBus.Messages.Common;
using HealthChecks.UI.Client;
using MassTransit;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Ordering.API.EventBusConsumer;
using Ordering.API.Swagger;
using Ordering.Application;
using Ordering.Infrastructure;
using Ordering.Infrastructure.Data;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Serilog configuration
builder.Host.UseSerilog(Logging.ConfigureLogger);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddApiVersioning();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddHealthChecks().Services.AddDbContext<OrderContext>();
builder.Services.AddScoped<ICorrelationIdGenerator, CorrelationIdGenerator>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddApiVersioning(opt =>
{
    opt.DefaultApiVersion = new ApiVersion(1, 0);
    opt.AssumeDefaultVersionWhenUnspecified = true;
    opt.ReportApiVersions = true;
    opt.ApiVersionReader = new UrlSegmentApiVersionReader();
}).AddMvc(options =>
{
    options.Conventions.Add(new VersionByNamespaceConvention());
}).AddApiExplorer(opt =>
{
    opt.GroupNameFormat = "'v'V";
    opt.SubstituteApiVersionInUrl = true;
});

builder.Services.ConfigureOptions<ConfigureSwaggerGenOptions>();


builder.Services.AddSwaggerGen(c =>
{
    c.IgnoreObsoleteActions();
    c.IgnoreObsoleteProperties();
});

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
    app.UseSwaggerUI(opt =>
    {
        var descriptions = app.DescribeApiVersions();

        foreach (var description in descriptions)
            opt.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", $"Order API {description.GroupName.ToLowerInvariant()}");
    });
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
