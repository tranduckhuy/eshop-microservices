using Basket.Application.Commands;
using Basket.Application.Mappers;
using Basket.Application.Queries;
using Basket.Application.Responses;
using Basket.Domain.Entities;
using EventBus.Messages.Events;
using MassTransit;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Basket.API.Controllers
{
    public class BasketController : BaseController
    {
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ILogger<BasketController> _logger;

        public BasketController(IMediator mediator, IPublishEndpoint publishEndpoint, ILogger<BasketController> logger) : base(mediator)
        {
            _publishEndpoint = publishEndpoint;
            _logger = logger;
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

        [HttpPost]
        [Route("[action]", Name = "Checkout")]
        public async Task<IActionResult> Checkout([FromBody] BasketCheckout basketCheckout)
        {
            var query = new GetBasketByUserNameQuery(basketCheckout.UserName);
            var result = await ExecuteAsync<GetBasketByUserNameQuery, BasketResponse>(query);

            if (result is not OkObjectResult okResult || okResult.Value is not ApiResponse<BasketResponse> apiResponse)
            {
                return BadRequest();
            }

            var eventMessage = BasketMapper.Mapper.Map<BasketCheckoutEvent>(basketCheckout);
            eventMessage.TotalPrice = apiResponse.Data!.TotalPrice;
            await _publishEndpoint.Publish(eventMessage);

            // Remove the basket
            var deleteCommand = new DeleteBasketByUserNameCommand(basketCheckout.UserName);
            await ExecuteAsync<DeleteBasketByUserNameCommand, bool>(deleteCommand);

            _logger.LogInformation("BasketCheckoutEvent published successfully. UserName: {UserName}", eventMessage.UserName);
            return Accepted();
        }

    }
}
