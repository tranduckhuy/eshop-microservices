using AutoMapper;
using EventBus.Messages.Events;
using Ordering.Application.Commands;
using Ordering.Application.Responses;
using Ordering.Domain.Entities;

namespace Ordering.Application.Mapper
{
    public class OrderingProfile : Profile
    {
        public OrderingProfile()
        {
            CreateMap<Order, CheckoutOrderCommand>().ReverseMap();
            CreateMap<Order, UpdateOrderCommand>().ReverseMap();
            CreateMap<Order, OrderResponse>().ReverseMap();
            CreateMap<BasketCheckoutEvent, CheckoutOrderCommand>().ReverseMap();
        }
    }
}
