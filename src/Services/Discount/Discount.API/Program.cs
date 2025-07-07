using Common.Logging;
using Common.Logging.Correlation;
using Discount.API.Services;
using Discount.Application;
using Discount.Infrastructure.Extensions;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Serilog configuration
builder.Host.UseSerilog(Logging.ConfigureLogger);

// Add services to the container.
builder.Services.AddGrpc();
builder.Services.AddInfrastructure();
builder.Services.AddApplication();
builder.Services.AddScoped<ICorrelationIdGenerator, CorrelationIdGenerator>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<DiscountService>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client.");

await app.RunAsync();
