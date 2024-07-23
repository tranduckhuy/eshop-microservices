using Basket.Domain.Entities;
using Basket.Domain.Repositories;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace Basket.Infrastructure.Repositories
{
    public class BasketRepository : IBasketRepository
    {
        private readonly IDistributedCache _redisCache;

        public BasketRepository(IDistributedCache redisCache)
        {
            _redisCache = redisCache;
        }

        public async Task<bool> DeleteBasket(string userName)
        {
            var basket = await GetBasket(userName);
            if (basket != null)
            {
                await _redisCache.RemoveAsync(userName);
                return true;
            }
            return false;
        }

        public async Task<Domain.Entities.ShoppingCart?> GetBasket(string userName)
        {
            var basket = await _redisCache.GetStringAsync(userName);
            if (string.IsNullOrEmpty(basket))
            {
                return null;
            }
            return JsonConvert.DeserializeObject<ShoppingCart>(basket);
        }

        public async Task<Domain.Entities.ShoppingCart?> UpdateBasket(Domain.Entities.ShoppingCart shoppingCart)
        {
            await _redisCache.SetStringAsync(shoppingCart.UserName, JsonConvert.SerializeObject(shoppingCart));
            return await GetBasket(shoppingCart.UserName);
        }
    }
}
