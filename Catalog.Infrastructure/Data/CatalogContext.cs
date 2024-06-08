using Catalog.Domain.Entities;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace Catalog.Infrastructure.Data
{
    public class CatalogContext : ICatalogContext
    {
        public IMongoCollection<Product> Products { get; }

        public IMongoCollection<Brand> Brands { get; }

        public IMongoCollection<Category> Categories { get; }

        public CatalogContext(IConfiguration configuration)
        {
            // Create a new instance of MongoClient and get the database
            var client = new MongoClient(configuration.GetValue<string>("DatabaseSettings:ConnectionString"));
            var database = client.GetDatabase(configuration.GetValue<string>("DatabaseSettings:DatabaseName"));

            Brands = database.GetCollection<Brand>(
                configuration.GetValue<string>("DatabaseSettings:BrandsCollection"));
            Categories = database.GetCollection<Category>(
                configuration.GetValue<string>("DatabaseSettings:CategoriesCollection"));
            Products = database.GetCollection<Product>(
                configuration.GetValue<string>("DatabaseSettings:ProductsCollection"));

            // Seed data
            BrandContextSeed.SeedData(Brands);
            CategoryContextSeed.SeedData(Categories);
            ProductContextSeed.SeedData(Products);
        }
    }
}
