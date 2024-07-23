namespace Ordering.Domain.Entities
{
    public abstract class EntityBase
    {
        // Id property is protected set, so it can be set only in derived classes
        public long Id { get; protected set; }

        // Audit properties
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; } = string.Empty;
        public DateTime LastModifiedDate { get; set; }
    }
}
