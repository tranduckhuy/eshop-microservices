using Catalog.API.Controllers.v1;
using Catalog.Application.Queries;
using Catalog.Application.Responses;
using Common.Logging.Correlation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace Catalog.API.Test
{
    [TestFixture]
    public class CategoriesControllerTests
    {
        private Mock<IMediator> _mediatorMock;
        private Mock<ILogger<CategoriesController>> _logger;
        private Mock<ICorrelationIdGenerator> _correlationIdGenerator;
        private CategoriesController _controller;

        [SetUp]
        public void SetUp()
        {
            _mediatorMock = new Mock<IMediator>();
            _logger = new Mock<ILogger<CategoriesController>>();
            _correlationIdGenerator = new Mock<ICorrelationIdGenerator>();
            _controller = new CategoriesController(_mediatorMock.Object, _logger.Object, _correlationIdGenerator.Object);
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
