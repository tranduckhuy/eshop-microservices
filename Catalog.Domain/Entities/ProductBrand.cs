using MongoDB.Bson.Serialization.Attributes;

namespace Catalog.Domain.Entities
{
    public class ProductBrand
    {
        [BsonElement("Name")]
        public string Name { get; set; } = string.Empty;
    }
}