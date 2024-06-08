using Catalog.Domain.Exceptions;

namespace Catalog.Application.Exceptions
{
    internal class CreateProductException : CatalogException
    {
        internal CreateProductException(string message) : base($"An error occured while creating product. {message}") { }
    }
}
