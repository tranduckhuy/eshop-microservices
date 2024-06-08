using Catalog.Domain.Exceptions;

namespace Catalog.Application.Exceptions
{
    internal class BrandNotFoundException : CatalogException
    {
        public BrandNotFoundException(Guid brandId) : base($"Brand with id {brandId} is not found.")
        {
        }
    }
}
