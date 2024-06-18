using Catalog.Application.Mappers;
using Catalog.Application.Responses;
using Catalog.Domain.Entities;
using Catalog.Domain.Repositories;
using MediatR;

namespace Catalog.Application.Queries.Handlers
{
    public class GetAllBrandsHandler : IRequestHandler<GetAllBrandsQuery, IEnumerable<BrandResponse>>
    {
        private readonly IBrandRepository _brandRepository;

        public GetAllBrandsHandler(IBrandRepository brandRepository)
        {
            _brandRepository = brandRepository;
        }

        public async Task<IEnumerable<BrandResponse>> Handle(GetAllBrandsQuery request, CancellationToken cancellationToken)
        {
            var brands = await _brandRepository.GetAll();
            return CatalogMapper.Mapper.Map<IEnumerable<Brand>, IEnumerable<BrandResponse>>(brands);
        }
    }
}
