using Catalog.Domain.Entities;

namespace Catalog.Domain.Repositories
{
    public interface IProducRepository
    {
        Task<IEnumerable<Product>> GetAll();
        Task<Product> GetById(Guid id);
        Task<IEnumerable<Product>> GetByName(string name);
        Task<IEnumerable<Product>> GetByBrand(string brand);
        Task<Product> Create(Product product);
        Task<bool> Update(Product product);
        Task<bool> DeleteById(Guid id);
    }
}
