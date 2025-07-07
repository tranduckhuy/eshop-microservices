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
    internal class BrandsControllerTests
    {
        private Mock<IMediator> _mediatorMock;
        private Mock<ILogger<BrandsController>> _logger;
        private Mock<ICorrelationIdGenerator> _correlationIdGenerator;
        private BrandsController _controller;

        [SetUp]
        public void SetUp()
        {
            _mediatorMock = new Mock<IMediator>();
            _logger = new Mock<ILogger<BrandsController>>();
            _correlationIdGenerator = new Mock<ICorrelationIdGenerator>();
            _controller = new BrandsController(_mediatorMock.Object, _logger.Object, _correlationIdGenerator.Object);
        }

        [Test]
        public async Task GetCategories_ReturnsOk()
        {
            // Arrange
            _mediatorMock.Setup(x => x.Send(It.IsAny<GetAllBrandsQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<BrandResponse>());

            // Act
            var result = await _controller.GetAllBrands();

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);
        }
    }
}
