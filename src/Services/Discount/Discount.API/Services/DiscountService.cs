using Common.Logging.Correlation;
using Discount.Application.Commands;
using Discount.Application.Queries;
using Discount.Grpc.Protos;
using Grpc.Core;
using MediatR;

namespace Discount.API.Services
{
    public class DiscountService : DiscountProtoService.DiscountProtoServiceBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<DiscountService> _logger;
        private readonly ICorrelationIdGenerator _correlationIdGenerator;

        public DiscountService(IMediator mediator, ILogger<DiscountService> logger, ICorrelationIdGenerator correlationIdGenerator)
        {
            _mediator = mediator;
            _logger = logger;
            _correlationIdGenerator = correlationIdGenerator;
            _logger.LogInformation("Correlation Id: {correlationId}", _correlationIdGenerator.Get());
        }

        public override async Task<CouponModel> GetDiscount(GetDiscountRequest request, ServerCallContext context)
        {
            var coupon = await _mediator.Send(new GetDiscountQuery(request.ProductName));
            _logger.LogInformation("Discount is retrieved for ProductName : {ProductName}, Amount : {Amount}", coupon.ProductName, coupon.Amount);
            return coupon;
        }

        public override async Task<CouponModel> CreateDiscount(CreateDiscountRequest request, ServerCallContext context)
        {
            var coupon = new CreateDiscountCommand(
                ProductName: request.Coupon.ProductName,
                Description: request.Coupon.Description,
                Amount: request.Coupon.Amount
           );

            var result = await _mediator.Send(coupon);
            _logger.LogInformation("Discount is successfully created. ProductName : {ProductName}", coupon.ProductName);
            return result;
        }

        public override async Task<DeleteDiscountResponse> DeleteDiscount(DeleteDiscountRequest request, ServerCallContext context)
        {
            var status = await _mediator.Send(new DeleteDiscountCommand(request.ProductName));
            _logger.LogInformation("Discount is successfully deleted. ProductName : {ProductName}", request.ProductName);
            var response = new DeleteDiscountResponse
            {
                Success = status
            };
            return response;
        }

        public override async Task<CouponModel> UpdateDiscount(UpdateDiscountRequest request, ServerCallContext context)
        {
            var coupon = new UpdateDiscountCommand(
                Id: request.Coupon.Id,
                ProductName: request.Coupon.ProductName,
                Description: request.Coupon.Description,
                Amount: request.Coupon.Amount
            );

            await _mediator.Send(coupon);
            _logger.LogInformation("Discount is successfully updated. ProductName : {ProductName}", coupon.ProductName);
            return request.Coupon;
        }
    }
}
