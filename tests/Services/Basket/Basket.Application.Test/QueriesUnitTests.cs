using Basket.Application.Commands;
using Basket.Application.Exceptions;
using Basket.Application.Queries;
using Basket.Application.Queries.Handlers;
using Basket.Domain.Entities;
using Basket.Domain.Repositories;
using Moq;

namespace Basket.Application.Test
{
    [TestFixture]
    public class QueriesUnitTests
    {
        private Mock<IBasketRepository> _basketRepository;
        private GetBasketByNameHandler _getBasketByNameHandler;

        [SetUp]
        public void Setup()
        {
            _basketRepository = new Mock<IBasketRepository>();
            _getBasketByNameHandler = new GetBasketByNameHandler(_basketRepository.Object);
        }

        [Test]
        public async Task Handle_BasketWithProvidedUserNameExist_ReturnsBasketResponses()
        {
            // Arrange
            var command = new GetBasketByUserNameQuery("user1");
            var basket = new ShoppingCart
            {
                UserName = command.UserName,
                Items = new List<ShoppingCartItem>
                {
                    new ShoppingCartItem { ProductId = Guid.NewGuid(), ProductName = "Product 1", ImageFile = "img1", Quantity = 1, Price = 123 },
                    new ShoppingCartItem { ProductId = Guid.NewGuid(), ProductName = "Product 2", ImageFile = "img2", Quantity = 2, Price = 456 }
                }
            };

            var totalPrice = basket.Items.Sum(x => x.Price * x.Quantity);

            _basketRepository.Setup(x => x.GetBasket(command.UserName)).ReturnsAsync(basket);

            // Act
            var result = await _getBasketByNameHandler.Handle(command, CancellationToken.None);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(result.UserName, Is.EqualTo(command.UserName));
                Assert.That(result.Items, Has.Count.EqualTo(basket.Items.Count));
                Assert.That(result.TotalPrice, Is.EqualTo(totalPrice));
            });
        }

        [Test]
        public void Handle_BasketWithProvidedUserNameDoesNotExist_ThrowsBasketNotFoundException()
        {
            // Arrange
            var command = new GetBasketByUserNameQuery("user1");

            _basketRepository.Setup(x => x.GetBasket(command.UserName)).ReturnsAsync((ShoppingCart?)null);

            // Act
            var exception = Assert.ThrowsAsync<BasketNotFoundException>(() => _getBasketByNameHandler.Handle(command, CancellationToken.None));

            // Assert
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Is.EqualTo($"Basket not found for user: '{command.UserName}'"));

            // Verify that GetBasket method was called once
            _basketRepository.Verify(x => x.GetBasket(command.UserName), Times.Once);
        }
    }
}
