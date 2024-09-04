using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using Serilog.Exceptions;
using Serilog.Sinks.Elasticsearch;

namespace Common.Logging
{
    public static class Logging
    {
        public static Action<HostBuilderContext, LoggerConfiguration> ConfigureLogger =>
            (context, loggerConfiguration) =>
            {
                var env = context.HostingEnvironment;
                loggerConfiguration.MinimumLevel.Information()
                    .Enrich.FromLogContext()
                    .Enrich.WithProperty("ApplicationName", env.ApplicationName)
                    .Enrich.WithProperty("EnvironmentName", env.EnvironmentName)
                    .Enrich.WithExceptionDetails()
                    .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
                    .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Warning)
                    .WriteTo.Console();

                if (context.HostingEnvironment.IsDevelopment())
                {
                    loggerConfiguration.MinimumLevel.Override("Catalog", LogEventLevel.Debug);
                    loggerConfiguration.MinimumLevel.Override("Basket", LogEventLevel.Debug);
                    loggerConfiguration.MinimumLevel.Override("Discount", LogEventLevel.Debug);
                    loggerConfiguration.MinimumLevel.Override("Ordering", LogEventLevel.Debug);
                }

                // Elasticsearch configuration
                var elasticUri = context.Configuration["ElasticConfiguration:Uri"];
                if (!string.IsNullOrWhiteSpace(elasticUri))
                {
                    loggerConfiguration.WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(elasticUri))
                    {
                        AutoRegisterTemplate = true,
                        AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv8,
                        IndexFormat = $"{env.ApplicationName?.ToLower().Replace(".", "-")}-{env.EnvironmentName?.ToLower().Replace(".", "-")}-{0:yyyy.MM.dd}",
                        MinimumLogEventLevel = LogEventLevel.Information
                    });
                }
            };
    }
}
