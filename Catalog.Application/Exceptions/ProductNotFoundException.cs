using Catalog.Domain.Exceptions;

namespace Catalog.Application.Exceptions
{
    internal class ProductNotFoundException : CatalogException
    {
        public ProductNotFoundException(string message)
            : base($"Product with {message} was not found.")
        {
        }
    }
}
