using Basket.Application.Commands;
using Basket.Application.Queries;
using Basket.Application.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Basket.API.Controllers
{
    public class BasketController : BaseController
    {
        public BasketController(IMediator mediator) : base(mediator)
        {
        }

        [HttpGet]
        [Route("User/{userName}", Name = "GetBasket")]
        public async Task<IActionResult> GetBasket(string userName)
        {
            var query = new GetBasketByUserNameQuery(userName);
            return await ExecuteAsync<GetBasketByUserNameQuery, BasketResponse>(query);
        }

        [HttpPost]
        [Route("[action]", Name = "UpdateBasket")]
        public async Task<IActionResult> UpdateBasket([FromBody] CreateBasketCommand command)
        {
            return await ExecuteAsync<CreateBasketCommand, BasketResponse>(command);
        }

        [HttpDelete]
        [Route("{userName}", Name = "DeleteBasket")]
        public async Task<IActionResult> DeleteBasket(string userName)
        {
            var command = new DeleteBasketByUserNameCommand(userName);
            return await ExecuteAsync<DeleteBasketByUserNameCommand, bool>(command);
        }
    }
}
