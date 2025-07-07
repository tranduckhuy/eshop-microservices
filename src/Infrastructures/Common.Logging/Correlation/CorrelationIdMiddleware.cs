using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace Common.Logging.Correlation
{
    public class CorrelationIdMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ICorrelationIdGenerator _correlationIdGenerator;
        private const string _correlationIdHeader = "X-Correlation-ID";

        public CorrelationIdMiddleware(RequestDelegate next, ICorrelationIdGenerator correlationIdGenerator)
        {
            _next = next;
            _correlationIdGenerator = correlationIdGenerator;
        }

        public async Task Invoke(HttpContext context)
        {
            var correlationId = GetCorrelationId(context, _correlationIdGenerator);
            AddCorrelationIdHeader(context, correlationId);

            await _next(context);
        }

        private static void AddCorrelationIdHeader(HttpContext context, StringValues correlationId)
        {
            context.Response.OnStarting(() =>
            {
                if (!context.Response.Headers.ContainsKey(_correlationIdHeader))
                {
                    context.Response.Headers.Append(_correlationIdHeader, correlationId);
                }

                return Task.CompletedTask;
            });
        }

        private static StringValues GetCorrelationId(HttpContext context, ICorrelationIdGenerator correlationIdGenerator)
        {
            if (context.Request.Headers.TryGetValue(_correlationIdHeader, out var correlationId))
            {
                correlationIdGenerator.Set(correlationId.ToString());
                return correlationId;
            }

            return correlationIdGenerator.Get();
        }
    }
}
