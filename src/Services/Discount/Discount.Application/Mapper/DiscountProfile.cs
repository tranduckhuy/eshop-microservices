using AutoMapper;
using Discount.Domain.Entities;
using Discount.Grpc.Protos;

namespace Discount.Application.Mapper
{
    public class DiscountProfile : Profile
    {
        public DiscountProfile()
        {
            CreateMap<Coupon, CouponModel>().ReverseMap();
        }
    }
}
