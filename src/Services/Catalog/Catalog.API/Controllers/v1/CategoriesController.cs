using Catalog.Application.Queries;
using Catalog.Application.Responses;
using Common.Logging.Correlation;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.API.Controllers.v1
{
    public class CategoriesController : BaseController<CategoriesController>
    {
        public CategoriesController(
            IMediator mediator,
            ILogger<CategoriesController> logger,
            ICorrelationIdGenerator correlationIdGenerator
        ) : base(mediator, logger, correlationIdGenerator) { }

        [HttpGet]
        public async Task<IActionResult> GetAllCategories()
        {
            var query = new GetAllCategoriesQuery();
            return await ExecuteAsync<GetAllCategoriesQuery, IEnumerable<CategoryResponse>>(query);
        }
    }
}
