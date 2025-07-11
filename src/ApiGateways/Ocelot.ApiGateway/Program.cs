using Common.Logging;
using Common.Logging.Correlation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Ocelot.Cache.CacheManager;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Configuration
builder.Configuration.AddJsonFile($"ocelot.{builder.Environment.EnvironmentName}.json", true, true);

// Serilog configuration
builder.Host.UseSerilog(Logging.ConfigureLogger);

// Add services to the container.
builder.Services.AddScoped<ICorrelationIdGenerator, CorrelationIdGenerator>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy",
        policy => { policy.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin(); });
});

// Add authentication
var authScheme = builder.Configuration["Authentication:AuthScheme"] ?? JwtBearerDefaults.AuthenticationScheme;
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(authScheme, jwtBearerOptions =>
    {
        jwtBearerOptions.Authority = builder.Configuration["Authentication:Authority"];
        jwtBearerOptions.Audience = builder.Configuration["Authentication:Audience"];

        jwtBearerOptions.TokenValidationParameters.ValidateAudience = true;
        jwtBearerOptions.TokenValidationParameters.ValidateIssuer = true;
        jwtBearerOptions.TokenValidationParameters.ValidateIssuerSigningKey = true;
    });

builder.Services.AddOcelot()
    .AddCacheManager(o => o.WithDictionaryHandle());

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

// Configure the HTTP request pipeline.
app.AddCorrelationIdMiddleware();

app.UseRouting();

app.UseHttpsRedirection();

app.UseCors("CorsPolicy");

app.MapGet("/", async context => { await context.Response.WriteAsync("Hello Ocelot"); });

await app.UseOcelot();

await app.RunAsync();
