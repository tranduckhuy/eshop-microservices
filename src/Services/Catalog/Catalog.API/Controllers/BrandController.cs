using Catalog.Application.Queries;
using Catalog.Application.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.API.Controllers
{
    public class BrandController : BaseController
    {
        public BrandController(IMediator mediator) : base(mediator)
        {
        }

        [HttpGet]
        [Route("GetAllProducts")]
        public async Task<IActionResult> GetAllBrands()
        {
            var query = new GetAllBrandsQuery();
            return await ExecuteAsync<GetAllBrandsQuery, IEnumerable<BrandResponse>>(query);
        }
    }
}
