using Catalog.Domain.Entities;

namespace Catalog.Domain.Repositories
{
    public interface ICategoryRepository
    {
        Task<IEnumerable<Category>> GetAll();
    }
}
