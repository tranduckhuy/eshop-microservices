using Catalog.Domain.Exceptions;

namespace Catalog.Application.Exceptions
{
    public class BrandNotFoundException : CatalogException
    {
        public Guid BrandId { get; set;  } 

        public BrandNotFoundException(Guid brandId) : base($"Brand with id {brandId} not found.")
        {
            BrandId = brandId;
        }
    }
}
