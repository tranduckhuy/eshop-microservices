using AutoMapper;

namespace Ordering.Application.Mapper
{
    public static class OrderingMapper
    {
        private readonly static Lazy<IMapper> _lazy = new Lazy<IMapper>(() =>
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.ShouldMapProperty = p => p.GetMethod?.IsPublic == true || p.GetMethod?.IsAssembly == true;
                cfg.AddProfile<OrderingProfile>();
            });

            return config.CreateMapper();
        });

        public static IMapper Mapper => _lazy.Value;
    }
}
