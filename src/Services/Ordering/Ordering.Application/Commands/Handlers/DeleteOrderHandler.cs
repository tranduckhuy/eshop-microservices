using MediatR;
using Microsoft.Extensions.Logging;
using Ordering.Application.Exceptions;
using Ordering.Domain.Entities;
using Ordering.Domain.Repositories;

namespace Ordering.Application.Commands.Handlers
{
    public class DeleteOrderHandler : IRequestHandler<DeleteOrderCommand, Unit>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ILogger<DeleteOrderHandler> _logger;

        public DeleteOrderHandler(IOrderRepository orderRepository, ILogger<DeleteOrderHandler> logger)
        {
            _orderRepository = orderRepository;
            _logger = logger;
        }

        public async Task<Unit> Handle(DeleteOrderCommand request, CancellationToken cancellationToken)
        {
            var order = await _orderRepository.GetByIdAsync(request.Id);
            if (order == null)
            {
                throw new OrderNotFoundException(nameof(Order), request.Id);
            }
            await _orderRepository.DeleteAsync(order);
            _logger.LogInformation("Order of '{UserName}' - 'Id: {Id}' is successfully deleted", order.UserName, order.Id);
            return Unit.Value;
        }
    }
}
