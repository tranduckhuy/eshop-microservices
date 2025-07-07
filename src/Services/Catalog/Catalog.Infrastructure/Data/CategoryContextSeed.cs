using Catalog.Domain.Entities;
using MongoDB.Driver;
using System.Text.Json;

namespace Catalog.Infrastructure.Data
{
    public static class CategoryContextSeed
    {
        public static void SeedData(IMongoCollection<Category> categoriesCollection)
        {
            // Check if there is already data in the collection
            bool existCategory = categoriesCollection.Find(_ => true).Any();

            if (!existCategory)
            {
                var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "SeedData", "categories.json");
                var categoriesData = File.ReadAllText(path);
                var categories = JsonSerializer.Deserialize<List<Category>>(categoriesData);

                if (categories != null)
                {
                    categoriesCollection.InsertManyAsync(categories);
                }
            }
        }
    }
}
