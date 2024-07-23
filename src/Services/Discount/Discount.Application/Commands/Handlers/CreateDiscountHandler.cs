using AutoMapper;
using Discount.Application.Exceptions;
using Discount.Application.Mapper;
using Discount.Domain.Entities;
using Discount.Domain.Repositories;
using Discount.Grpc.Protos;
using MediatR;

namespace Discount.Application.Commands.Handlers
{
    public class CreateDiscountHandler : IRequestHandler<CreateDiscountCommand, CouponModel>
    {
        private readonly IDiscountRepository _repository;

        public CreateDiscountHandler(IDiscountRepository repository)
        {
            _repository = repository;
        }

        public async Task<CouponModel> Handle(CreateDiscountCommand request, CancellationToken cancellationToken)
        {
            var coupon = DiscountMapper.Mapper.Map<Coupon>(request);
            var result = await _repository.CreateDiscount(coupon);
            if (result != null)
            {
                return DiscountMapper.Mapper.Map<CouponModel>(result);
            }
            throw new CreateDiscountException(coupon.ProductName);
        }
    }   
}
