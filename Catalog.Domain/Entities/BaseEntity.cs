using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Catalog.Domain.Entitys
{
    public abstract class BaseEntity
    {
        [BsonId]
        public Guid Id { get; set; } = Guid.NewGuid();
    }
}
