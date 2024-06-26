using Basket.Application.Exceptions;
using Basket.Application.Mappers;
using Basket.Application.Responses;
using Basket.Domain.Entities;
using Basket.Domain.Repositories;
using MediatR;

namespace Basket.Application.Queries.Handlers
{
    public class GetBasketByNameHandler : IRequestHandler<GetBasketByUserNameQuery, BasketResponse>
    {
        private readonly IBasketRepository _repository;

        public GetBasketByNameHandler(IBasketRepository repository)
        {
            _repository = repository;
        }

        public async Task<BasketResponse> Handle(GetBasketByUserNameQuery request, CancellationToken cancellationToken)
        {
            var basket = await _repository.GetBasket(request.UserName);
            if (basket != null)
            {
                return BasketMapper.Mapper.Map<ShoppingCart, BasketResponse>(basket);
            }
            throw new BasketNotFoundException(request.UserName);    
        }
    }
}
