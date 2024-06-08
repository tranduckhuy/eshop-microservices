using Catalog.Domain.Exceptions;

namespace Catalog.Application.Exceptions
{
    internal class CategoryNotFoundException : CatalogException
    {
        public CategoryNotFoundException(Guid categoryId) : base($"Category with id {categoryId} not found.")
        {
        }
    }
}
