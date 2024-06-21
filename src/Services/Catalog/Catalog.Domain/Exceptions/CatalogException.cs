namespace Catalog.Domain.Exceptions
{
    public abstract class CatalogException : Exception
    {
        protected CatalogException(string message) : base(message) { }
    }
}


