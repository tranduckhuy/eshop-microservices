namespace Basket.Domain.Exceptions
{
    public class BasketException : Exception
    {
        protected BasketException(string message) : base(message) { }
    }
}
