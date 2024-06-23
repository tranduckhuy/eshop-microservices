using Basket.Application.Mappers;
using Basket.Application.Responses;
using Basket.Domain.Entities;
using Basket.Domain.Repositories;
using MediatR;

namespace Basket.Application.Commands.Handlers
{
    public class CreateShoppingCartHandler : IRequestHandler<CreateShoppingCartCommand, ShoppingCartResponse>
    {
        private readonly IBasketRepository _basketRepository;

        public CreateShoppingCartHandler(IBasketRepository basketRepository)
        {
            _basketRepository = basketRepository;
        }

        public async Task<ShoppingCartResponse> Handle(CreateShoppingCartCommand request, CancellationToken cancellationToken)
        {
            // TODO: Call Discount.Grpc and apply coupons

            var shoppingCart = new ShoppingCart
            {
                UserName = request.UserName,
                Items = request.Items
            };

            var result = await _basketRepository.UpdateBasket(shoppingCart);
            return BasketMapper.Mapper.Map<ShoppingCartResponse>(result);
        }
    }
}
