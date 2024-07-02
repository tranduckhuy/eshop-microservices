using AutoMapper;
using Discount.Application.Commands;
using Discount.Domain.Entities;
using Discount.Grpc.Protos;

namespace Discount.Application.Mapper
{
    public class DiscountProfile : Profile
    {
        public DiscountProfile()
        {
            CreateMap<Coupon, CouponModel>().ReverseMap();
            CreateMap<Coupon, CreateDiscountCommand>().ReverseMap();
            CreateMap<Coupon, UpdateDiscountCommand>().ReverseMap();
        }
    }
}
