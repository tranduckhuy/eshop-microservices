using Catalog.Domain.Exceptions;

namespace Catalog.Application.Exceptions
{
    internal class UpdateProductException : CatalogException
    {
        internal UpdateProductException(string message) : base($"An error occured while updating product. {message}") { }
    }
}
