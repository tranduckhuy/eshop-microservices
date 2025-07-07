namespace Ordering.Domain.Exceptions
{
    public class OrderingException : Exception
    {
        protected OrderingException(string message) : base(message) { }
    }
}
