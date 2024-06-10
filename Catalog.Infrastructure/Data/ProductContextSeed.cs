using Catalog.Domain.Entities;
using MongoDB.Driver;
using System.Text.Json;

namespace Catalog.Infrastructure.Data
{
    public static class ProductContextSeed
    {
        public static void SeedData(IMongoCollection<Product> productsCollection)
        {
            // Check if there is already data in the collection
            bool existProduct = productsCollection.Find(_ => true).Any();

            if (!existProduct)
            {
                var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "SeedData", "products.json");
                var productsData = File.ReadAllText(path);
                var products = JsonSerializer.Deserialize<List<Product>>(productsData);

                if (products != null)
                {
                    productsCollection.InsertManyAsync(products);
                }
            }
        }

    }
}
