using AutoMapper;
using Discount.Application.Exceptions;
using Discount.Application.Mapper;
using Discount.Domain.Entities;
using Discount.Domain.Repositories;
using Discount.Grpc.Protos;
using MediatR;

namespace Discount.Application.Commands.Handlers
{
    public class UpdateDiscountHandler : IRequestHandler<UpdateDiscountCommand, bool>
    {
        private readonly IDiscountRepository _repository;

        public UpdateDiscountHandler(IDiscountRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> Handle(UpdateDiscountCommand request, CancellationToken cancellationToken)
        {
            var coupon = DiscountMapper.Mapper.Map<Coupon>(request);
            var result = await _repository.UpdateDiscount(coupon);
            return result ? result : throw new DiscountNotFountException(coupon.ProductName);
        }
    }
}
