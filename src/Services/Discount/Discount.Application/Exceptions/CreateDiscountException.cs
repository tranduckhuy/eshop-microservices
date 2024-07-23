using Grpc.Core;

namespace Discount.Application.Exceptions
{
    public class CreateDiscountException : RpcException
    {
        public string ProductName { get; private set; }

        public CreateDiscountException(string productName)
            : base(new Status(StatusCode.AlreadyExists, $"Failed to create a discount for {productName}."))
        {
            ProductName = productName;
        }

    }
}
