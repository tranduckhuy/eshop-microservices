using Basket.Application.Commands;
using Basket.Application.Mappers;
using Basket.Application.Queries;
using Basket.Application.Responses;
using Basket.Domain.Entities;
using Common.Logging.Correlation;
using EventBus.Messages.Events;
using MassTransit;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Basket.API.Controllers.v1
{
    public class BasketController : BaseController<BasketController>
    {
        private readonly IPublishEndpoint _publishEndpoint;

        public BasketController(
            IMediator mediator,
            IPublishEndpoint publishEndpoint,
            ILogger<BasketController> logger,
            ICorrelationIdGenerator correlationIdGenerator
        ) : base(mediator, logger, correlationIdGenerator)
        {
            _publishEndpoint = publishEndpoint;
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
        [Route("User/{userName}", Name = "DeleteBasket")]
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
                Logger.LogError("Error while getting basket by username. UserName: {UserName}", basketCheckout.UserName);
                return BadRequest();
            }

            var eventMessage = BasketMapper.Mapper.Map<BasketCheckoutEvent>(basketCheckout);
            eventMessage.TotalPrice = apiResponse.Data!.TotalPrice;
            eventMessage.CorrelationId = CorrelationIdGenerator.Get();
            await _publishEndpoint.Publish(eventMessage);

            // Remove the basket
            var deleteCommand = new DeleteBasketByUserNameCommand(basketCheckout.UserName);
            await ExecuteAsync<DeleteBasketByUserNameCommand, bool>(deleteCommand);

            Logger.LogInformation("BasketCheckoutEvent published successfully. UserName: {UserName}", eventMessage.UserName);
            return Accepted();
        }

    }
}
