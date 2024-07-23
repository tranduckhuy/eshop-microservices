using Basket.Application.Exceptions;
using Basket.Domain.Repositories;
using MediatR;

namespace Basket.Application.Commands.Handlers
{
    public class DeleteBasketByUserNameHandler : IRequestHandler<DeleteBasketByUserNameCommand, bool>
    {
        private readonly IBasketRepository _repository;

        public DeleteBasketByUserNameHandler(IBasketRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> Handle(DeleteBasketByUserNameCommand request, CancellationToken cancellationToken)
        {
            return await _repository.DeleteBasket(request.UserName) ? true : throw new DeleteBasketException(request.UserName);
        }
    }
}
