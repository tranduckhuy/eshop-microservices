using AutoMapper;
using Discount.Application.Commands;
using Discount.Application.Commands.Handlers;
using Discount.Application.Exceptions;
using Discount.Domain.Entities;
using Discount.Domain.Repositories;
using Moq;
using System;

namespace Discount.Application.Test
{
    [TestFixture]
    public class CommandUnitTests
    {
        private Mock<IDiscountRepository> _mockDiscountRepository;
        private CreateDiscountHandler _createDiscountCommandHandler;
        private DeleteDiscountHandler _deleteDiscountCommandHandler;
        private UpdateDiscountHandler _updateDiscountCommandHandler;

        [SetUp]
        public void Setup()
        {
            _mockDiscountRepository = new Mock<IDiscountRepository>();
            _createDiscountCommandHandler = new CreateDiscountHandler(_mockDiscountRepository.Object);
            _deleteDiscountCommandHandler = new DeleteDiscountHandler(_mockDiscountRepository.Object);
            _updateDiscountCommandHandler = new UpdateDiscountHandler(_mockDiscountRepository.Object);
        }

        #region CreateDiscountCommand
        [Test]
        public async Task HandleCreate_ShouldCreateDiscount_WhenAllValidationsPass()
        {
            // Arrange
            var command = new CreateDiscountCommand
            (
                ProductName: "Product 1",
                Description: "Description 1",
                Amount: 10
            );

            _mockDiscountRepository.Setup(x => x.CreateDiscount(It.IsAny<Coupon>())).ReturnsAsync(new Coupon
            {
                ProductName = command.ProductName,
                Description = command.Description,
                Amount = command.Amount
            });

            // Act
            var result = await _createDiscountCommandHandler.Handle(command, CancellationToken.None);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(result.ProductName, Is.EqualTo(command.ProductName));
                Assert.That(result.Amount, Is.EqualTo(command.Amount));
            });

            _mockDiscountRepository.Verify(x => x.CreateDiscount(It.IsAny<Coupon>()), Times.Once);
        }

        [Test]
        public void HandleCreate_ShouldThrowException_WhenDiscountAlreadyExists()
        {
            // Arrange
            var command = new CreateDiscountCommand
            (
                ProductName: "Product 1",
                Description: "Description 1",
                Amount: 10
            );

            _mockDiscountRepository.Setup(x => x.CreateDiscount(It.IsAny<Coupon>())).ReturnsAsync((Coupon?) null);

            // Act
            var result = Assert.ThrowsAsync<CreateDiscountException>(() => _createDiscountCommandHandler.Handle(command, CancellationToken.None));

            // Assert
            StringAssert.Contains($"Failed to create a discount for {command.ProductName}.", result.Message);

            _mockDiscountRepository.Verify(x => x.CreateDiscount(It.IsAny<Coupon>()), Times.Once);
        }
        #endregion

        #region DeleteDiscountCommand
        [Test]
        public async Task HandleDelete_ShouldDeleteDiscount_WhenDiscountExists()
        {
            // Arrange
            var command = new DeleteDiscountCommand("Product 1");

            _mockDiscountRepository.Setup(x => x.DeleteDiscount(It.IsAny<string>())).ReturnsAsync(true);

            // Act
            var result = await _deleteDiscountCommandHandler.Handle(command, CancellationToken.None);

            // Assert
            Assert.That(result, Is.True);

            _mockDiscountRepository.Verify(x => x.DeleteDiscount(It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void HandleDelete_ShouldThrowException_WhenDiscountDoesNotExist()
        {
            // Arrange
            var command = new DeleteDiscountCommand("Product 1");

            _mockDiscountRepository.Setup(x => x.DeleteDiscount(It.IsAny<string>())).ReturnsAsync(false);

            // Act
            var result = Assert.ThrowsAsync<DiscountNotFountException>(() => _deleteDiscountCommandHandler.Handle(command, CancellationToken.None));

            // Assert
            StringAssert.Contains($"Discount for product '{command.ProductName}' was not found", result.Message);

            _mockDiscountRepository.Verify(x => x.DeleteDiscount(It.IsAny<string>()), Times.Once);
        }

        #endregion

        #region UpdateDiscountCommand
        [Test]
        public async Task HandleUpdate_ShouldUpdateDiscount_WhenDiscountExists()
        {
            // Arrange
            var command = new UpdateDiscountCommand
            (
                Id: 1,
                ProductName: "Product 1",
                Description: "Description 1",
                Amount: 10
            );

            _mockDiscountRepository.Setup(x => x.UpdateDiscount(It.IsAny<Coupon>())).ReturnsAsync(true);

            // Act
            var result = await _updateDiscountCommandHandler.Handle(command, CancellationToken.None);

            // Assert
            Assert.That(result, Is.True);

            _mockDiscountRepository.Verify(x => x.UpdateDiscount(It.IsAny<Coupon>()), Times.Once);
        }

        [Test]
        public void HandleUpdate_ShouldThrowException_WhenDiscountDoesNotExist()
        {
            // Arrange
            var command = new UpdateDiscountCommand
            (
                Id: 1,
                ProductName: "Product 1",
                Description: "Description 1",
                Amount: 10
            );

            _mockDiscountRepository.Setup(x => x.UpdateDiscount(It.IsAny<Coupon>())).ReturnsAsync(false);

            // Act
            var result = Assert.ThrowsAsync<DiscountNotFountException>(() => _updateDiscountCommandHandler.Handle(command, CancellationToken.None));

            // Assert
            StringAssert.Contains($"Discount for product '{command.ProductName}' was not found", result.Message);

            _mockDiscountRepository.Verify(x => x.UpdateDiscount(It.IsAny<Coupon>()), Times.Once);
        }
        #endregion
    }
}
