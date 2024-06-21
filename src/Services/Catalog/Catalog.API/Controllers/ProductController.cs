using Catalog.Application.Commands;
using Catalog.Application.Queries;
using Catalog.Application.Responses;
using Catalog.Domain.Specs;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.API.Controllers
{
    public class ProductController : BaseController
    {
        public ProductController(IMediator mediator) : base(mediator)
        {
        }

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
        [Route("GetProducts")]
        public async Task<IActionResult> GetProducts([FromQuery] CatalogSpecParams catalogSpecParams)
        {
            var query = new GetProductsQuery(catalogSpecParams);
            return await ExecuteAsync<GetProductsQuery, Pagination<ProductResponse>>(query);
        }

        [HttpPost("UpdateProduct")]
        public async Task<IActionResult> UpdateProduct([FromBody] UpdateProductCommand command)
        {
            return await ExecuteAsync<UpdateProductCommand, bool>(command);
        }

        [HttpPost]
        [Route("CreateProduct")]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductCommand command)
        {
            return await ExecuteAsync<CreateProductCommand, ProductResponse>(command);
        }

        [HttpGet]
        [Route("[action]/{id:guid}", Name = "DeleteProduct")]
        public async Task<IActionResult> DeleteProduct(Guid id)
        {
            var command = new DeleteProductByIdCommand(id);
            return await ExecuteAsync<DeleteProductByIdCommand, bool>(command);
        }
    }
}
