using AutoMapper;
using Discount.Application.Exceptions;
using Discount.Domain.Entities;
using Discount.Domain.Repositories;
using Discount.Grpc.Protos;
using MediatR;

namespace Discount.Application.Commands.Handlers
{
    public class CreateDiscountHandler : IRequestHandler<CreateDiscountCommand, CouponModel>
    {
        private readonly IDiscountRepository _repository;
        private readonly IMapper _mapper;

        public CreateDiscountHandler(IDiscountRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<CouponModel> Handle(CreateDiscountCommand request, CancellationToken cancellationToken)
        {
            var coupon = _mapper.Map<Coupon>(request);
            var result = await _repository.CreateDiscount(coupon);
            if (result != null)
            {
                return _mapper.Map<CouponModel>(result);
            }
            throw new CreateDiscountException(coupon.ProductName);
        }
    }   
}
