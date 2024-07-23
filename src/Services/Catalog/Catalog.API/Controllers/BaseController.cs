using Asp.Versioning;
using Catalog.Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Catalog.API.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public abstract class BaseController : ControllerBase
    {
        private readonly IMediator _mediator;

        protected BaseController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Validate request
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private BadRequestObjectResult ValidateRequest<TRequest, TResponse>(TRequest request)
            where TRequest : class, IRequest<TResponse>
        {

            if (request == null)
            {
                return BadRequest(new ApiResponse<TResponse>
                {
                    IsSuccess = false,
                    Message = "Request cannot be null!"
                });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<TRequest>
                {
                    IsSuccess = false,
                    Message = "Invalid request",
                    Errors = ModelState
                });
            }

            return null!;
        }

        /// <summary>
        /// Handle the execution of the request
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        protected async Task<IActionResult> ExecuteAsync<TRequest, TResponse>(TRequest request)
            where TRequest : class, IRequest<TResponse>
        {
            var validationResult = ValidateRequest<TRequest, TResponse>(request);
            if (validationResult != null)
            {
                return validationResult;
            }

            try
            {
                var response = await _mediator.Send(request);
                return Ok(
                    new ApiResponse<TResponse>
                    {
                        IsSuccess = true,
                        Data = response,
                        Message = "Request processed successfully!"
                    }
                );
            }
            catch (Exception ex)
            {
                return HandleError<TResponse>(ex);
            }
        }

        /// <summary>
        /// Handle errors
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        private ObjectResult HandleError<TResponse>(Exception ex)
        {
            // Log the exception if needed
            var statusCode = HttpStatusCode.InternalServerError;
            var message = "An error occurred while processing the request";

            if (ex is CatalogException)
            {
                statusCode = HttpStatusCode.BadRequest;
            }

            // Log the exception if needed
            var errorResponse = new ApiResponse<TResponse>
            {
                IsSuccess = false,
                Message = message,
                Details = ex.Message
            };
            return StatusCode((int)statusCode, errorResponse);
        }
    }
}
