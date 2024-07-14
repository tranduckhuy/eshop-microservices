using MediatR;
using Microsoft.Extensions.Logging;
using Ordering.Application.Mapper;
using Ordering.Domain.Entities;
using Ordering.Domain.Repositories;

namespace Ordering.Application.Commands.Handlers
{
    public class CheckoutOrderHandler : IRequestHandler<CheckoutOrderCommand, long>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ILogger<CheckoutOrderHandler> _logger;

        public CheckoutOrderHandler(IOrderRepository orderRepository, ILogger<CheckoutOrderHandler> logger)
        {
            _orderRepository = orderRepository;
            _logger = logger;
        }

        public async Task<long> Handle(CheckoutOrderCommand request, CancellationToken cancellationToken)
        {
            var order = OrderingMapper.Mapper.Map<Order>(request);
            var generateOrder = await _orderRepository.AddAsync(order);
            _logger.LogInformation($"----- Order Created 'Username: {generateOrder.UserName}' - 'Id: {generateOrder.Id}'");
            return generateOrder.Id;
        }
    }
}
