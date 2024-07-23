using Discount.Application.Exceptions;
using Discount.Application.Queries;
using Discount.Application.Queries.Handlers;
using Discount.Domain.Entities;
using Discount.Domain.Repositories;
using Moq;

namespace Discount.Application.Test
{
    [TestFixture]
    public class QueriesUnitTests
    {
        private Mock<IDiscountRepository> _mockDiscountRepository;
        private GetDiscountHandler _getDiscountQueryHandler;

        [SetUp]
        public void Setup()
        {
            _mockDiscountRepository = new Mock<IDiscountRepository>();
            _getDiscountQueryHandler = new GetDiscountHandler(_mockDiscountRepository.Object);
        }

        #region GetDiscountQuery
        [Test]
        public async Task Handle_ShouldReturnDiscount_WhenDiscountExists()
        {
            // Arrange
            var discount = new Coupon
            {
                ProductName = "Product 1",
                Description = "Description 1",
                Amount = 10
            };

            _mockDiscountRepository.Setup(x => x.GetDiscount(It.IsAny<string>())).ReturnsAsync(discount);

            // Act
            var result = await _getDiscountQueryHandler.Handle(new GetDiscountQuery("Product 1"), CancellationToken.None);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(result.ProductName, Is.EqualTo(discount.ProductName));
                Assert.That(result.Amount, Is.EqualTo(discount.Amount));
            });

            _mockDiscountRepository.Verify(x => x.GetDiscount(It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void Handle_ShouldThrow_DiscountNotFountException_WhenDiscountDoesNotExist()
        {
            // Arrange
            _mockDiscountRepository.Setup(x => x.GetDiscount(It.IsAny<string>())).ReturnsAsync((Coupon?)null);

            // Act
            var result = Assert.ThrowsAsync<DiscountNotFountException>(() => _getDiscountQueryHandler.Handle(new GetDiscountQuery("Product 1"), CancellationToken.None));

            // Assert
            StringAssert.Contains($"Discount for product '{result.ProductName}' was not found", result.Message);

            _mockDiscountRepository.Verify(x => x.GetDiscount(It.IsAny<string>()), Times.Once);
        }
        #endregion
    }
}
