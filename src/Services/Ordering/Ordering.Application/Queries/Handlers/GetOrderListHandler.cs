using MediatR;
using Ordering.Application.Exceptions;
using Ordering.Application.Mapper;
using Ordering.Application.Responses;
using Ordering.Domain.Entities;
using Ordering.Domain.Repositories;

namespace Ordering.Application.Queries.Handlers
{
    public class GetOrderListHandler : IRequestHandler<GetOrdersByUserNameQuery, IEnumerable<OrderResponse>>
    {
        private readonly IOrderRepository _orderRepository;

        public GetOrderListHandler(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<IEnumerable<OrderResponse>> Handle(GetOrdersByUserNameQuery request, CancellationToken cancellationToken)
        {
            var orderList = await _orderRepository.GetOrdersByUserName(request.UserName);
            if (orderList != null)
            {
                return OrderingMapper.Mapper.Map<IEnumerable<OrderResponse>>(orderList);
            }
            throw new OrderNotFoundException(nameof(Order), request.UserName);
        }
    }
}
