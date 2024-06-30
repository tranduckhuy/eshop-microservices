using Grpc.Core;

namespace Discount.Application.Exceptions
{
    public class DiscountNotFountException : RpcException
    {
        public string ProductName { get; private set; }
        public DiscountNotFountException(string productName) 
            : base(new Status(StatusCode.NotFound, $"Discount for product '{productName}' was not found"))
        {
            ProductName = productName;
        }
    }
}
