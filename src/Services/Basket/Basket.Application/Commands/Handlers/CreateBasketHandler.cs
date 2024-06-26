using Basket.Application.Mappers;
using Basket.Application.Responses;
using Basket.Domain.Entities;
using Basket.Domain.Repositories;
using MediatR;

namespace Basket.Application.Commands.Handlers
{
    public class CreateBasketHandler : IRequestHandler<CreateBasketCommand, BasketResponse>
    {
        private readonly IBasketRepository _basketRepository;

        public CreateBasketHandler(IBasketRepository basketRepository)
        {
            _basketRepository = basketRepository;
        }

        public async Task<BasketResponse> Handle(CreateBasketCommand request, CancellationToken cancellationToken)
        {
            // TODO: Call Discount.Grpc and apply coupons

            var shoppingCart = new Domain.Entities.ShoppingCart
            {
                UserName = request.UserName,
                Items = request.Items
            };

            var result = await _basketRepository.UpdateBasket(shoppingCart);
            return BasketMapper.Mapper.Map<BasketResponse>(result);
        }
    }
}
