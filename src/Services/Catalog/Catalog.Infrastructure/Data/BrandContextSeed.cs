using Catalog.Domain.Entities;
using MongoDB.Driver;
using System.Text.Json;

namespace Catalog.Infrastructure.Data
{
    public static class BrandContextSeed
    {
        public static void SeedData(IMongoCollection<Brand> brandsCollection)
        {
            // Check if there is already data in the collection
            bool existBrand = brandsCollection.Find(_ => true).Any();

            if (!existBrand)
            {
                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "SeedData", "brands.json");
                var brandsData = File.ReadAllText(path);
                var brands = JsonSerializer.Deserialize<List<Brand>>(brandsData);

                if (brands != null)
                {
                    brandsCollection.InsertManyAsync(brands);
                }
            }
        }
    }
}
