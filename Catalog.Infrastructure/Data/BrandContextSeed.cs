using Catalog.Domain.Entities;
using MongoDB.Driver;
using System.Text.Json;

namespace Catalog.Infrastructure.Data
{
    public static class BrandContextSeed
    {
        public static void SeedData(IMongoCollection<Brand> brandCollection)
        {
            // Check if there is already data in the collection
            bool existBrand = brandCollection.Find(p => true).Any();

            if (!existBrand)
            {
                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "SeedData", "brands.js");
                var brandsData = File.ReadAllText(path);
                var brands = JsonSerializer.Deserialize<List<Brand>>(brandsData);

                if (brands != null)
                {
                    brandCollection.InsertManyAsync(brands);
                }
            }
        }
    }
}
