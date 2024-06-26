using Basket.Application.Commands;
using Basket.Application.Commands.Handlers;
using Basket.Application.Exceptions;
using Basket.Domain.Entities;
using Basket.Domain.Repositories;
using Moq;

namespace Basket.Application.Test;

[TestFixture]
public class CommandUnitTests
{
    private Mock<IBasketRepository> _mockBasketRepository;
    private CreateBasketHandler _createBasketCommandHandler;
    private DeleteBasketByUserNameHandler _deleteBasketCommandHandler;

    [SetUp]
    public void Setup()
    {
        _mockBasketRepository = new Mock<IBasketRepository>();
        _createBasketCommandHandler = new CreateBasketHandler(_mockBasketRepository.Object);
        _deleteBasketCommandHandler = new DeleteBasketByUserNameHandler(_mockBasketRepository.Object);
    }

    #region CreateShoppingCartCommand

    [Test]
    public async Task Handle_ShouldCreateCart_WhenAllValidationsPass()
    {
        // Arrange
        var command = new CreateBasketCommand
        {
            UserName = "user1",
            Items = new List<ShoppingCartItem>
            {
                new ShoppingCartItem { ProductId = Guid.NewGuid(), ProductName = "Product 1", ImageFile = "img1", Quantity = 1, Price = 123 },
                new ShoppingCartItem { ProductId = Guid.NewGuid(), ProductName = "Product 2", ImageFile = "img2", Quantity = 2, Price = 456 }
            }
        };

        _mockBasketRepository.Setup(x => x.UpdateBasket(It.IsAny<ShoppingCart>())).ReturnsAsync(new ShoppingCart
        {
            UserName = command.UserName,
            Items = command.Items
        });

        // Act
        var result = await _createBasketCommandHandler.Handle(command, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result.UserName, Is.EqualTo(command.UserName));
            Assert.That(result.Items, Has.Count.EqualTo(command.Items.Count));
        });

        _mockBasketRepository.Verify(x => x.UpdateBasket(It.IsAny<ShoppingCart>()), Times.Once);
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