using Basket.Application.Commands;
using Basket.Application.Commands.Handlers;
using Basket.Application.Exceptions;
using Basket.Application.GrpcServices;
using Basket.Domain.Entities;
using Basket.Domain.Repositories;
using Discount.Grpc.Protos;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Moq;

namespace Basket.Application.Test;

[TestFixture]
public class CommandUnitTests
{
    private Mock<IBasketRepository> _mockBasketRepository;
    private Mock<DiscountProtoService.DiscountProtoServiceClient> _discountProtoServiceClientMock;
    private Mock<ILogger<CreateBasketHandler>> _loggerMock;
    private DiscountGrpcService _discountGrpcService;
    private CreateBasketHandler _createBasketHandler;
    private DeleteBasketByUserNameHandler _deleteBasketCommandHandler;

    [SetUp]
    public void SetUp()
    {
        _mockBasketRepository = new Mock<IBasketRepository>();
        _loggerMock = new Mock<ILogger<CreateBasketHandler>>();
        _discountProtoServiceClientMock = new Mock<DiscountProtoService.DiscountProtoServiceClient>();
        _discountGrpcService = new DiscountGrpcService(_discountProtoServiceClientMock.Object);
        _createBasketHandler = new CreateBasketHandler(_mockBasketRepository.Object, _discountGrpcService, _loggerMock.Object);
        _deleteBasketCommandHandler = new DeleteBasketByUserNameHandler(_mockBasketRepository.Object);
    }

    private static AsyncUnaryCall<CouponModel> CreateAsyncUnaryCall(CouponModel response)
    {
        var tcs = new TaskCompletionSource<CouponModel>();
        tcs.SetResult(response);
        return new AsyncUnaryCall<CouponModel>(
            tcs.Task,
            Task.FromResult(new Metadata()),
            () => Status.DefaultSuccess,
            () => new Metadata(),
            () => { });
    }

    #region CreateBasketCommand
    [Test]
    public async Task Handle_WithValidCommand_ReturnsBasketResponse()
    {
        // Arrange
        var command = new CreateBasketCommand
        {
            UserName = "testuser",
            Items = new List<ShoppingCartItem>
            {
                new ShoppingCartItem { ProductName = "Product1", Price = 100 },
                new ShoppingCartItem { ProductName = "Product2", Price = 200 }
            }
        };

        var coupon1 = new CouponModel { ProductName = "Product1", Amount = 10 };
        var coupon2 = new CouponModel { ProductName = "Product2", Amount = 20 };

        _discountProtoServiceClientMock
            .Setup(client => client.GetDiscountAsync(
                It.Is<GetDiscountRequest>(r => r.ProductName == "Product1"),
                null, null, default))
            .Returns(CreateAsyncUnaryCall(coupon1));

        _discountProtoServiceClientMock
            .Setup(client => client.GetDiscountAsync(
                It.Is<GetDiscountRequest>(r => r.ProductName == "Product2"),
                null, null, default))
            .Returns(CreateAsyncUnaryCall(coupon2));

        var updatedShoppingCart = new ShoppingCart
        {
            UserName = command.UserName,
            Items = new List<ShoppingCartItem>
            {
                new ShoppingCartItem { ProductName = "Product1", Price = 90 },
                new ShoppingCartItem { ProductName = "Product2", Price = 180 }
            }
        };

        _mockBasketRepository
            .Setup(r => r.UpdateBasket(It.IsAny<ShoppingCart>()))
            .ReturnsAsync(updatedShoppingCart);

        // Act
        var result = await _createBasketHandler.Handle(command, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result.UserName, Is.EqualTo("testuser"));
            Assert.That(result.Items, Has.Count.EqualTo(2));
        });
        Assert.Multiple(() =>
        {
            Assert.That(result.Items[0].Price, Is.EqualTo(90));
            Assert.That(result.Items[1].Price, Is.EqualTo(180));
        });

        _mockBasketRepository.Verify(r => r.UpdateBasket(It.IsAny<ShoppingCart>()), Times.Once);
        _discountProtoServiceClientMock.Verify(client => client.GetDiscountAsync(
            It.Is<GetDiscountRequest>(r => r.ProductName == "Product1"),
            null, null, default), Times.Once);
        _discountProtoServiceClientMock.Verify(client => client.GetDiscountAsync(
            It.Is<GetDiscountRequest>(r => r.ProductName == "Product2"),
            null, null, default), Times.Once);
    }

    #endregion

    #region DeleteBasketByUserNameCommand

    [Test]
    public async Task Handle_ShouldDeleteCart_WhenAllValidationsPass()
    {
        // Arrange
        var command = new DeleteBasketByUserNameCommand("user1");

        _mockBasketRepository.Setup(x => x.DeleteBasket(It.IsAny<string>())).ReturnsAsync(true);

        // Act
        var result = await _deleteBasketCommandHandler.Handle(command, CancellationToken.None);

        // Assert
        Assert.That(result, Is.True);
        _mockBasketRepository.Verify(x => x.DeleteBasket(It.IsAny<string>()), Times.Once);
    }

    [Test]
    public void Handle_ShouldThrowException_WhenBasketIsNotFound()
    {
        // Arrange
        var command = new DeleteBasketByUserNameCommand("user1");

        _mockBasketRepository.Setup(x => x.DeleteBasket(It.IsAny<string>())).ReturnsAsync(false);

        // Act
        var ex = Assert.ThrowsAsync<DeleteBasketException>(() => _deleteBasketCommandHandler.Handle(command, CancellationToken.None));

        // Assert
        Assert.That(ex.Message, Is.EqualTo($"Basket for user '{command.UserName}' could not be deleted"));
        _mockBasketRepository.Verify(x => x.DeleteBasket(It.IsAny<string>()), Times.Once);
    }

    #endregion
}