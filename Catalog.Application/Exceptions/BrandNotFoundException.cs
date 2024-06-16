using Catalog.Domain.Exceptions;

namespace Catalog.Application.Exceptions
{
    public class BrandNotFoundException : CatalogException
    {
        public BrandNotFoundException(Guid brandId) : base($"Brand with id {brandId} not found.")
        {
        }
    }
}
