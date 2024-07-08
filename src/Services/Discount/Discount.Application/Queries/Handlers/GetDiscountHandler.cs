using Discount.Application.Exceptions;
using Discount.Application.Mapper;
using Discount.Domain.Repositories;
using Discount.Grpc.Protos;
using MediatR;

namespace Discount.Application.Queries.Handlers
{
    public class GetDiscountHandler : IRequestHandler<GetDiscountQuery, CouponModel>
    {
        private readonly IDiscountRepository _repository;

        public GetDiscountHandler(IDiscountRepository repository)
        {
            _repository = repository;
        }

        public async Task<CouponModel> Handle(GetDiscountQuery request, CancellationToken cancellationToken)
        {
            var coupon = await _repository.GetDiscount(request.ProductName);
            if (coupon != null)
            {
                return DiscountMapper.Mapper.Map<CouponModel>(coupon);
            }
            throw new DiscountNotFountException(request.ProductName);
        }
    }
}
