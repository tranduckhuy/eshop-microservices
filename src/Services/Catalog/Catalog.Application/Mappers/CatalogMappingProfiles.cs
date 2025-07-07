﻿using AutoMapper;
using Catalog.Application.Commands;
using Catalog.Application.Responses;
using Catalog.Domain.Entities;
using Catalog.Domain.Specs;

namespace Catalog.Application.Mappers
{
    public class CatalogMappingProfiles : Profile
    {
        public CatalogMappingProfiles()
        {
            CreateMap<Brand, BrandResponse>().ReverseMap();
            CreateMap<Product, ProductResponse>().ReverseMap();
            CreateMap<Category, CategoryResponse>().ReverseMap();

            CreateMap<CreateProductCommand, Product>()
               .ForMember(dest => dest.Id, opt => opt.Ignore())
               .ForMember(dest => dest.Brand, opt => opt.Ignore())
               .ForMember(dest => dest.Category, opt => opt.Ignore());

            CreateMap<UpdateProductCommand, Product>()
               .ForMember(dest => dest.Brand, opt => opt.Ignore())
               .ForMember(dest => dest.Category, opt => opt.Ignore());

            CreateMap<Pagination<Product>, Pagination<ProductResponse>>().ReverseMap();
        }
    }
}
