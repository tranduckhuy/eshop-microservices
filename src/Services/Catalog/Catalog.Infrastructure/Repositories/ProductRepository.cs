using Catalog.Domain.Entities;
using Catalog.Domain.Repositories;
using Catalog.Domain.Specs;
using Catalog.Infrastructure.Data;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Catalog.Infrastructure.Repositories
{
    public class ProductRepository(ICatalogContext context) : IProductRepository
    {
        private readonly ICatalogContext _context = context;

        public async Task<Product> Create(Product product)
        {
            await _context.Products.InsertOneAsync(product);
            return product;
        }

        public async Task<bool> DeleteById(Guid id)
        {
            FilterDefinition<Product> filter = Builders<Product>.Filter.Eq(p => p.Id, id);
            DeleteResult deleteResult = await _context.Products.DeleteOneAsync(filter);
            return deleteResult.IsAcknowledged && deleteResult.DeletedCount > 0;
        }

        public async Task<Pagination<Product>> GetProducts(CatalogSpecParams catalogSpecParams)
        {
            var builder = Builders<Product>.Filter;
            var filter = builder.Empty;
            if (!string.IsNullOrEmpty(catalogSpecParams.Search))
            {
                var searchFilter = builder.Regex(x => x.Name, new BsonRegularExpression(catalogSpecParams.Search));
                filter &= searchFilter;
            }
            if (catalogSpecParams.BrandId.HasValue)
            {
                var brandFilter = builder.Eq(x => x.Brand!.Id, catalogSpecParams.BrandId.Value);
                filter &= brandFilter;
            }

            if (catalogSpecParams.CategoryId.HasValue)
            {
                var typeFilter = builder.Eq(x => x.Category!.Id, catalogSpecParams.CategoryId);
                filter &= typeFilter;
            }

            if (!string.IsNullOrEmpty(catalogSpecParams.Sort))
            {
                return new Pagination<Product>
                {
                    PageIndex = catalogSpecParams.PageIndex,
                    PageSize = catalogSpecParams.PageSize,
                    Data = await DataFilter(catalogSpecParams, filter),
                    Count = await _context.Products.CountDocumentsAsync(filter)
                };
            }

            return new Pagination<Product>
            {
                PageSize = catalogSpecParams.PageSize,
                PageIndex = catalogSpecParams.PageIndex,
                Data = await _context
                    .Products
                    .Find(filter)
                    .Sort(Builders<Product>.Sort.Ascending("Name"))
                    .Skip(catalogSpecParams.PageSize * (catalogSpecParams.PageIndex - 1))
                    .Limit(catalogSpecParams.PageSize)
                    .ToListAsync(),
                Count = await _context.Products.CountDocumentsAsync(p => true)
            };
        }

        private async Task<IReadOnlyCollection<Product>> DataFilter(CatalogSpecParams catalogSpecParams, FilterDefinition<Product> filter)
        {
            var sortField = "Name";
            if (catalogSpecParams.Sort == "priceAsc" || catalogSpecParams.Sort == "priceDesc")
            {
                sortField = "Price";
            }

            var sortDefinition = Builders<Product>.Sort.Ascending(sortField);

            if (catalogSpecParams.Sort == "priceDesc")
            {
                sortDefinition = Builders<Product>.Sort.Descending(sortField);
            }

            var skip = catalogSpecParams.PageSize * (catalogSpecParams.PageIndex - 1);
            var limit = catalogSpecParams.PageSize;

            var products = await _context.Products
                .Find(filter)
                .Sort(sortDefinition)
                .Skip(skip)
                .Limit(limit)
                .ToListAsync();

            return products;
        }

        public async Task<Product?> GetById(Guid id)
        {
            return await _context.Products.Find(p => p.Id == id).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Product>> GetByName(string name)
        {
            var filter = Builders<Product>.Filter.Regex(p => p.Name, new BsonRegularExpression(name, "i"));
            return await _context.Products.Find(filter).ToListAsync();
        }

        public async Task<bool> Update(Product product)
        {
            ReplaceOneResult updateResult = await _context.Products
                .ReplaceOneAsync(filter: p => p.Id == product.Id, replacement: product);

            return updateResult.IsAcknowledged && updateResult.ModifiedCount > 0;
        }
    }
}
