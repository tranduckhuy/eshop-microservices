using Catalog.Domain.Exceptions;

namespace Catalog.Application.Exceptions
{
    public class CategoryNotFoundException : CatalogException
    {
        public Guid CategoryId { get; set; }

        public CategoryNotFoundException(Guid categoryId) : base($"Category with id {categoryId} not found.")
        {
            CategoryId = categoryId;
        }
    }
}
