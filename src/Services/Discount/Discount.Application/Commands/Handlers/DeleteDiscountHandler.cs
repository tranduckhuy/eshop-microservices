using AutoMapper;
using Discount.Application.Exceptions;
using Discount.Domain.Repositories;
using MediatR;

namespace Discount.Application.Commands.Handlers
{
    public class DeleteDiscountHandler : IRequestHandler<DeleteDiscountCommand, bool>
    {
        private readonly IDiscountRepository _repository;

        public DeleteDiscountHandler(IDiscountRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> Handle(DeleteDiscountCommand request, CancellationToken cancellationToken)
        {
            var deleted = await _repository.DeleteDiscount(request.ProductName);
            return deleted ? deleted : throw new DiscountNotFountException(request.ProductName);
        }
    }
}
