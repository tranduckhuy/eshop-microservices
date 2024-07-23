using Ordering.Domain.Exceptions;

namespace Ordering.Application.Exceptions
{
    public class OrderNotFoundException(string name, object key) 
        : OrderingException($"Entity {name} - {key} is not found.")
    {
    }
}
