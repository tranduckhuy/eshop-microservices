using Azure;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Ordering.Application.Commands;
using Ordering.Application.Queries;
using Ordering.Application.Responses;

namespace Ordering.API.Controllers
{
    public class OrdersController : BaseController
    {
        public OrdersController(IMediator mediator) : base(mediator)
        {
        }

        [HttpGet]
        [Route("User/{userName}", Name = "GetOrdersByUserName")]
        public async Task<IActionResult> GetOrdersByUserName(string userName)
        {
            var query = new GetOrdersByUserNameQuery(userName);
            return await ExecuteAsync<GetOrdersByUserNameQuery, IEnumerable<OrderResponse>>(query);
        }

        [HttpPost]
        [Route("Checkout")]
        public async Task<IActionResult> CheckoutOrder([FromBody] CheckoutOrderCommand command)
        {
            return await ExecuteAsync<CheckoutOrderCommand, long>(command);
        }

        [HttpPut]
        [Route("Update/{id:long}")]
        public async Task<IActionResult> UpdateOrder([FromBody] UpdateOrderCommand command, long id)
        {
            if (command.Id != id)
            {
                var errorResponse = new ApiResponse<OrderResponse>
                {
                    IsSuccess = false,
                    Message = "The provided ID does not match the resource ID.",
                    Details = "The ID provided in the request body does not match the ID specified in the URL."
                };
                return BadRequest(errorResponse);
            }
            return await ExecuteAsync<UpdateOrderCommand, Unit>(command);
        }

        [HttpDelete]
        [Route("{id:long}")]
        public async Task<IActionResult> DeleteOrder(long id)
        {
            var command = new DeleteOrderCommand(id);
            return await ExecuteAsync<DeleteOrderCommand, Unit>(command);
        }
    }
}
