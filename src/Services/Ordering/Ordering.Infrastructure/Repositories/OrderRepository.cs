using Microsoft.EntityFrameworkCore;
using Ordering.Domain.Entities;
using Ordering.Domain.Repositories;
using Ordering.Infrastructure.Data;

namespace Ordering.Infrastructure.Repositories
{
    public class OrderRepository : RepositoryBase<Order>, IOrderRepository
    {
        public OrderRepository(OrderContext orderContext) : base(orderContext) {}

        public async Task<IEnumerable<Order>> GetOrdersByUserName(string userName)
        {
            return await _orderContext.Orders
                .Where(o => o.UserName == userName)
                .OrderByDescending(o => o.CreatedDate)
                .ToListAsync();
        }
    }
}
