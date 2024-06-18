using Catalog.Domain.Entities;
using Catalog.Domain.Repositories;
using Catalog.Infrastructure.Data;
using MongoDB.Driver;

namespace Catalog.Infrastructure.Repositories
{
    public class BrandRepository(ICatalogContext context) : IBrandRepository
    {
        private readonly ICatalogContext _context = context;

        public async Task<IEnumerable<Brand>> GetAll()
            => await _context.Brands.Find(_ => true).ToListAsync();

        public async Task<Brand?> GetById(Guid Id)
        {
            FilterDefinition<Brand> filter = Builders<Brand>.Filter.Eq(b => b.Id, Id);
            return await _context.Brands.Find(filter).FirstOrDefaultAsync();
        }
    }
}
