using AutoMapper;
using Catalog.Application.Responses;
using Catalog.Domain.Entities;

namespace Catalog.Application.Mappers
{
    public class CatalogMappingProfiles : Profile
    {
        public CatalogMappingProfiles()
        {
            CreateMap<Brand, BrandResponse>().ReverseMap();
            CreateMap<Product, ProductResponse>().ReverseMap();
            CreateMap<Category, CategoryResponse>().ReverseMap();
        }
    }
}
