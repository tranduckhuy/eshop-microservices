using Catalog.Application.Queries;
using Catalog.Application.Responses;
using Common.Logging.Correlation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.API.Controllers.v1
{
    public class BrandsController : BaseController<BrandsController>
    {
        public BrandsController(
            IMediator mediator,
            ILogger<BrandsController> logger,
            ICorrelationIdGenerator correlationIdGenerator
        ) : base(mediator, logger, correlationIdGenerator) { }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAllBrands()
        {
            var query = new GetAllBrandsQuery();
            return await ExecuteAsync<GetAllBrandsQuery, IEnumerable<BrandResponse>>(query);
        }
    }
}
