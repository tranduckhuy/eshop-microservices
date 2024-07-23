using Basket.API.Controllers;
using Basket.Application.Commands;
using Basket.Application.Exceptions;
using Basket.Application.Queries;
using Basket.Application.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Basket.API.Test
{
    [TestFixture]
    public class BasketControllerTests
    {
        private Mock<IMediator> _mediatorMock;
        private BasketController _controller;

        [SetUp]
        public void SetUp()
        {
            _mediatorMock = new Mock<IMediator>();
            _controller = new BasketController(_mediatorMock.Object);
        }

        [Test]
        public async Task GetBasket_ReturnsBasketResponse()
        {
            // Arrange
            var userName = "test";
            var query = new GetBasketByUserNameQuery(userName);
            var response = new BasketResponse { UserName = userName };
            _mediatorMock.Setup(x => x.Send(It.IsAny<GetBasketByUserNameQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            // Act
            var result = await _controller.GetBasket(userName);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);
            var apiResponse = okResult.Value as ApiResponse<BasketResponse>;
            Assert.That(apiResponse, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(apiResponse.IsSuccess, Is.True);
                Assert.That(apiResponse.Data, Is.EqualTo(response));
            });
        }

        [Test]
        public async Task UpdateBasket_ReturnsBasketResponse()
        {
            // Arrange
            var command = new CreateBasketCommand();
            var response = new BasketResponse { UserName = "test" };
            _mediatorMock.Setup(x => x.Send(It.IsAny<CreateBasketCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            // Act
            var result = await _controller.UpdateBasket(command);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);
            var apiResponse = okResult.Value as ApiResponse<BasketResponse>;
            Assert.That(apiResponse, Is.Not.Null);
            Assert.That(apiResponse.IsSuccess, Is.True);
        }

        [TestCase("Huydeptrai")]
        [TestCase("Thucsu")]
        [Test]
        public async Task GetBasketByUserName_ReturnsApiResponseWithError_WhenBasketNotFound(string name)
        {
            // Arrange
            var productName = name;
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetBasketByUserNameQuery>(), default))
                .ThrowsAsync(new BasketNotFoundException($"{name}"));

            // Act
            var result = await _controller.GetBasket(name);

            // Assert
            Assert.That(result, Is.InstanceOf<ObjectResult>());
            var objectResult = result as ObjectResult;
            Assert.That(objectResult, Is.Not.Null);
            Assert.That(objectResult.StatusCode, Is.EqualTo(400));

            var apiResponse = objectResult.Value as ApiResponse<BasketResponse>;
            Assert.That(apiResponse, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(apiResponse.IsSuccess, Is.False);
                Assert.That(apiResponse.Details, Is.EqualTo($"Basket not found for user: '{name}'"));
                Assert.That(apiResponse.Data, Is.Null);
            });
        }

        [Test]
        public async Task GetBasketByUserName_ReturnsApiResponseWithError_WhenModelIsInvalid()
        {
            // Arrange
            string name = "huydz"; // Example name

            // Simulate invalid ModelState
            _controller.ModelState.AddModelError("PropertyName", "Error message");

            // Act
            var result = await _controller.GetBasket(name);

            // Assert
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
            var badRequestResult = result as BadRequestObjectResult;
            Assert.That(badRequestResult, Is.Not.Null);

            var apiResponse = badRequestResult.Value as ApiResponse<GetBasketByUserNameQuery>;
            Assert.That(apiResponse, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(apiResponse.IsSuccess, Is.False);
                Assert.That(apiResponse.Message, Is.EqualTo("Invalid request"));
                Assert.That(apiResponse.Errors, Is.Not.Null); // Ensure ModelState errors are captured
            });
        }

        [Test]
        public async Task UpdateBasket_ReturnsApiResponseWithError_WhenModelIsNull()
        {
            // Act
            var result = await _controller.UpdateBasket(null!);

            // Assert
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
            var badRequestResult = result as BadRequestObjectResult;
            Assert.That(badRequestResult, Is.Not.Null);

            var apiResponse = badRequestResult.Value as ApiResponse<BasketResponse>;
            Assert.That(apiResponse, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(apiResponse.IsSuccess, Is.False);
                Assert.That(apiResponse.Message, Is.EqualTo("Request cannot be null!"));
            });
        }
    }
}