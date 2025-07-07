using Microsoft.EntityFrameworkCore;
using Ordering.Domain.Entities;
using Ordering.Domain.Repositories;
using Ordering.Infrastructure.Data;
using System.Linq.Expressions;

namespace Ordering.Infrastructure.Repositories
{
    public class RepositoryBase<T> : IAsyncRepository<T> where T : EntityBase
    {
        protected readonly OrderContext _orderContext;

        public RepositoryBase(OrderContext orderContext)
        {
            _orderContext = orderContext;
        }

        public async Task<IReadOnlyList<T>> GetAllAsync()
        {
            return await _orderContext.Set<T>().ToListAsync();
        }

        public async Task<IReadOnlyList<T>> GetAsync(Expression<Func<T, bool>> predicate)
        {
            return await _orderContext.Set<T>().Where(predicate).ToListAsync();
        }

        public async Task<T?> GetByIdAsync(long id)
        {
            return await _orderContext.Set<T>().FindAsync(id);
        }

        public async Task<T> AddAsync(T entity)
        {
            _orderContext.Set<T>().Add(entity);
            await _orderContext.SaveChangesAsync();
            return entity;
        }

        public async Task DeleteAsync(T entity)
        {
            _orderContext.Set<T>().Remove(entity);
            await _orderContext.SaveChangesAsync();
        }

        public Task UpdateAsync(T entity)
        {
            _orderContext.Entry(entity).State = EntityState.Modified;
            return _orderContext.SaveChangesAsync();
        }
    }
}
