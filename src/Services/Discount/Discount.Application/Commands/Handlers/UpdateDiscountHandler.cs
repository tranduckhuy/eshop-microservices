using AutoMapper;
using Discount.Application.Exceptions;
using Discount.Domain.Entities;
using Discount.Domain.Repositories;
using Discount.Grpc.Protos;
using MediatR;

namespace Discount.Application.Commands.Handlers
{
    public class UpdateDiscountHandler : IRequestHandler<UpdateDiscountCommand, bool>
    {
        private readonly IDiscountRepository _repository;
        private readonly IMapper _mapper;

        public UpdateDiscountHandler(IDiscountRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<bool> Handle(UpdateDiscountCommand request, CancellationToken cancellationToken)
        {
            var coupon = _mapper.Map<Coupon>(request);
            var result = await _repository.UpdateDiscount(coupon);
            return result ? result : throw new DiscountNotFountException(coupon.ProductName);
        }
    }
}
