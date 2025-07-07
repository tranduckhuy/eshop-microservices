using Basket.Domain.Exceptions;

namespace Basket.Application.Exceptions
{
    public class DeleteBasketException : BasketException
    {
        public string UserName { get; set; }    
        public DeleteBasketException(string userName) : base($"Basket for user '{userName}' could not be deleted")
        {
            UserName = userName;
        }
    }
}
