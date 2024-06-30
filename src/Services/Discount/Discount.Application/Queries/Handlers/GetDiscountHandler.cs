using AutoMapper;
using Discount.Application.Exceptions;
using Discount.Domain.Repositories;
using Discount.Grpc.Protos;
using MediatR;

namespace Discount.Application.Queries.Handlers
{
    public class GetDiscountHandler : IRequestHandler<GetDiscountQuery, CouponModel>
    {
        private readonly IDiscountRepository _repository;
        private readonly IMapper _mapper;

        public GetDiscountHandler(IDiscountRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<CouponModel> Handle(GetDiscountQuery request, CancellationToken cancellationToken)
        {
            var coupon = await _repository.GetDiscount(request.ProductName);
            if (coupon != null)
            {
                return _mapper.Map<CouponModel>(coupon);
            }
            throw new DiscountNotFountException(request.ProductName);
        }
    }
}
