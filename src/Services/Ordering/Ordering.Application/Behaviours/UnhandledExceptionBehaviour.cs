using MediatR;
using Microsoft.Extensions.Logging;
using Ordering.Application.Exceptions;

namespace Ordering.Application.Behaviours
{
    public class UnhandledExceptionBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        private readonly ILogger<UnhandledExceptionBehaviour<TRequest, TResponse>> _logger;

        public UnhandledExceptionBehaviour(ILogger<UnhandledExceptionBehaviour<TRequest, TResponse>> logger)
        {
            _logger = logger;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            try
            {
                return await next();
            }
            catch (Exception e)
            {
                var requestName = typeof(TRequest).Name;
                _logger.LogError(e, "Unhandled Exception Occurred with Request Name: {RequestName}, Request: {Request}", requestName, request);
                throw new RequestHandlingException(requestName, $"An error occurred while handling the request: {requestName}", e);
            }
        }
    }
}
