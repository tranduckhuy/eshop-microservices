using Catalog.Domain.Entities;
using MongoDB.Driver;

namespace Catalog.Infrastructure.Data
{
    public interface ICatalogContext
    {
        IMongoCollection<Product> Products { get; } 
        IMongoCollection<Brand> Brands { get; } 
        IMongoCollection<Category> Categories { get; } 
    }
}
