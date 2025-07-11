﻿using Catalog.Application.Commands;
using Catalog.Application.Queries;
using Catalog.Application.Responses;
using Catalog.Domain.Specs;
using Common.Logging.Correlation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.API.Controllers.v1
{
    public class ProductsController : BaseController<ProductsController>
    {
        public ProductsController(
            IMediator mediator,
            ILogger<ProductsController> logger,
            ICorrelationIdGenerator correlationIdGenerator
        ) : base(mediator, logger, correlationIdGenerator) { }

        [HttpGet]
        [Route("{id:guid}", Name = "GetProductById")]
        public async Task<IActionResult> GetProductById(Guid id)
        {
            var query = new GetProductByIdQuery(id);
            return await ExecuteAsync<GetProductByIdQuery, ProductResponse>(query);
        }

        [HttpGet]
        [Route("[action]/{name}", Name = "GetProductByProductName")]
        public async Task<IActionResult> GetProductByName(string name)
        {
            var query = new GetProductByNameQuery(name);
            return await ExecuteAsync<GetProductByNameQuery, IEnumerable<ProductResponse>>(query);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetProducts([FromQuery] CatalogSpecParams catalogSpecParams)
        {
            var query = new GetProductsQuery(catalogSpecParams);
            return await ExecuteAsync<GetProductsQuery, Pagination<ProductResponse>>(query);
        }

        [HttpPut]
        [Route("{id:guid}")]
        public async Task<IActionResult> UpdateProduct([FromBody] UpdateProductCommand command, Guid id)
        {
            if (command.Id != id)
            {
                var errorResponse = new ApiResponse<ProductResponse>
                {
                    IsSuccess = false,
                    Message = "The provided ID does not match the resource ID.",
                    Details = "The ID provided in the request body does not match the ID specified in the URL."
                };
                return BadRequest(errorResponse);
            }
            return await ExecuteAsync<UpdateProductCommand, bool>(command);
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductCommand command)
        {
            return await ExecuteAsync<CreateProductCommand, ProductResponse>(command);
        }

        [HttpDelete]
        [Route("{id:guid}", Name = "DeleteProduct")]
        public async Task<IActionResult> DeleteProduct(Guid id)
        {
            var command = new DeleteProductByIdCommand(id);
            return await ExecuteAsync<DeleteProductByIdCommand, bool>(command);
        }
    }
}
