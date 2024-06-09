using Catalog.Application.Queries;
using Catalog.Application.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.API.Controllers
{
    public class CategoryController : BaseController
    {
        public CategoryController(IMediator mediator) : base(mediator)
        {
        }

        [HttpGet]
        [Route("GetAllCategories")]
        public async Task<IActionResult> GetAllCategories([FromQuery] GetAllCategoriesQuery query)
        {
            return await ExecuteAsync<GetAllCategoriesQuery, IEnumerable<CategoryResponse>>(query);
        }
    }
}
