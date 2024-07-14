using MediatR;
using Microsoft.Extensions.Logging;
using Ordering.Application.Exceptions;
using Ordering.Application.Mapper;
using Ordering.Domain.Entities;
using Ordering.Domain.Repositories;

namespace Ordering.Application.Commands.Handlers
{
    public class UpdateOrderHandler : IRequestHandler<UpdateOrderCommand, Unit>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ILogger<UpdateOrderHandler> _logger;

        public UpdateOrderHandler(IOrderRepository orderRepository, ILogger<UpdateOrderHandler> logger)
        {
            _orderRepository = orderRepository;
            _logger = logger;
        }

        public async Task<Unit> Handle(UpdateOrderCommand request, CancellationToken cancellationToken)
        {
            var existingOrder = await _orderRepository.GetByIdAsync(request.Id);
            if (existingOrder == null)
            {
                throw new OrderNotFoundException(nameof(Order), request.Id);
            }
            OrderingMapper.Mapper.Map<UpdateOrderCommand, Order>(request, existingOrder);
            await _orderRepository.UpdateAsync(existingOrder);
            _logger.LogInformation($"Order of '{existingOrder.UserName}' - 'Id: {existingOrder.Id}' is successfully updated");

            return Unit.Value;
        }
    }
}
