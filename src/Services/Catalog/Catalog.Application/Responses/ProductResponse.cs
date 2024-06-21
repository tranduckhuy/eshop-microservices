using Catalog.Domain.Entities;

namespace Catalog.Application.Responses
{
    public class ProductResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Summary { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ImageFile { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public Brand? Brand { get; set; }
        public Category? Category { get; set; }
    }
}
