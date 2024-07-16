using Catalog.API.Controllers;
using Catalog.Application.Queries;
using Catalog.Application.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Catalog.API.Test
{
    [TestFixture]
    public class CategoriesControllerTests
    {
        private Mock<IMediator> _mediatorMock;
        private CategoriesController _controller;

        [SetUp]
        public void SetUp()
        {
            _mediatorMock = new Mock<IMediator>();
            _controller = new CategoriesController(_mediatorMock.Object);
        }

        [Test]
        public async Task GetCategories_ReturnsOk()
        {
            // Arrange
            _mediatorMock.Setup(x => x.Send(It.IsAny< GetAllCategoriesQuery> (), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<CategoryResponse>());

            // Act
            var result = await _controller.GetAllCategories();

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);
        }
    }
}
