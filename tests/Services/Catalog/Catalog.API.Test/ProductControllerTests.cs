using Catalog.API.Controllers;
using Catalog.Application.Commands;
using Catalog.Application.Exceptions;
using Catalog.Application.Queries;
using Catalog.Application.Responses;
using Catalog.Domain.Specs;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;
using Moq;

namespace Catalog.API.Tests.Controllers
{
    [TestFixture]
    public class ProductControllerTests
    {
        private Mock<IMediator> _mediatorMock;
        private ProductController _controller;

        [SetUp]
        public void SetUp()
        {
            _mediatorMock = new Mock<IMediator>();
            _controller = new ProductController(_mediatorMock.Object);
        }

        [Test]
        public async Task GetProductById_ReturnsProduct_WhenProductExists()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var productResponse = new ProductResponse { Id = productId };
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetProductByIdQuery>(), default))
                .ReturnsAsync(productResponse);

            // Act
            var result = await _controller.GetProductById(productId);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);

            var apiResponse = okResult.Value as ApiResponse<ProductResponse>;
            Assert.That(apiResponse, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(apiResponse.IsSuccess, Is.True);
                Assert.That(apiResponse.Data, Is.EqualTo(productResponse));
            });
        }

        [Test]
        public async Task GetProductByName_ReturnsProducts_WhenProductsExist()
        {
            // Arrange
            var productName = "TestProduct";
            var products = new List<ProductResponse> { new ProductResponse { Id = Guid.NewGuid(), Name = productName } };
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetProductByNameQuery>(), default))
                .ReturnsAsync(products);

            // Act
            var result = await _controller.GetProductByName(productName);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);

            var apiResponse = okResult.Value as ApiResponse<IEnumerable<ProductResponse>>;
            Assert.That(apiResponse, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(apiResponse.IsSuccess, Is.True);
                Assert.That(apiResponse.Data, Is.EqualTo(products));
            });
        }

        [Test]
        public async Task GetProducts_ReturnsPagedProducts()
        {
            // Arrange
            var catalogSpecParams = new CatalogSpecParams();
            var pagedProducts = new Pagination<ProductResponse>(0, 1, 10, new List<ProductResponse>());
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetProductsQuery>(), default))
                .ReturnsAsync(pagedProducts);

            // Act
            var result = await _controller.GetProducts(catalogSpecParams);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);
            var apiResponse = okResult.Value as ApiResponse<Pagination<ProductResponse>>;
            Assert.That(apiResponse, Is.Not.Null);
            Assert.That(apiResponse.Data, Is.EqualTo(pagedProducts));
        }

        [Test]
        public async Task UpdateProduct_ReturnsTrue_WhenUpdateIsSuccessful()
        {
            // Arrange
            var updateProductCommand = CreateValidUpdateProductCommand();
            _mediatorMock.Setup(m => m.Send(It.IsAny<UpdateProductCommand>(), default))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.UpdateProduct(updateProductCommand);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);
            var apiResponse = okResult.Value as ApiResponse<bool>;
            Assert.That(apiResponse, Is.Not.Null);
            Assert.That(apiResponse.Data, Is.EqualTo(true));
        }

        [Test]
        public async Task CreateProduct_ReturnsCreatedProduct()
        {
            // Arrange
            var createProductCommand = CreateValidCreateProductCommand();
            var createdProductResponse = new ProductResponse();
            _mediatorMock.Setup(m => m.Send(It.IsAny<CreateProductCommand>(), default))
                .ReturnsAsync(createdProductResponse);

            // Act
            var result = await _controller.CreateProduct(createProductCommand);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);

            var apiResponse = okResult.Value as ApiResponse<ProductResponse>;
            Assert.That(apiResponse, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(apiResponse.IsSuccess, Is.True);
                Assert.That(apiResponse.Data, Is.EqualTo(createdProductResponse));
            });
        }

        [Test]
        public async Task DeleteProduct_ReturnsTrue_WhenDeleteIsSuccessful()
        {
            // Arrange
            var productId = Guid.NewGuid();
            _mediatorMock.Setup(m => m.Send(It.IsAny<DeleteProductByIdCommand>(), default))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.DeleteProduct(productId);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);
            var apiResponse = okResult.Value as ApiResponse<bool>;
            Assert.That(apiResponse, Is.Not.Null);
            Assert.That(apiResponse.Data, Is.EqualTo(true));
        }

        [TestCase("Huydeptrai")]
        [TestCase("Thucsu")]
        [Test]
        public async Task GetProductByName_ReturnsApiResponseWithError_WhenProductNotFound(string name)
        {
            // Arrange
            var productName = name;
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetProductByNameQuery>(), default))
                .ThrowsAsync(new ProductNotFoundException($"name '{name}'"));

            // Act
            var result = await _controller.GetProductByName(name);

            // Assert
            Assert.That(result, Is.InstanceOf<ObjectResult>());
            var objectResult = result as ObjectResult;
            Assert.That(objectResult, Is.Not.Null);
            Assert.That(objectResult.StatusCode, Is.EqualTo(400));

            var apiResponse = objectResult.Value as ApiResponse<IEnumerable<ProductResponse>>;
            Assert.That(apiResponse, Is.Not.Null);
            Assert.That(apiResponse.IsSuccess, Is.False);
            Assert.That(apiResponse.Details, Is.EqualTo($"Product with name '{name}' was not found."));
            Assert.That(apiResponse.Data, Is.Null);
        }

        [Test]
        public async Task GetProductByName_ReturnsApiResponseWithError_WhenModelIsInvalid()
        {
            // Arrange
            string name = "Adidas"; // Example name

            // Simulate invalid ModelState
            _controller.ModelState.AddModelError("PropertyName", "Error message");

            // Act
            var result = await _controller.GetProductByName(name);

            // Assert
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
            var badRequestResult = result as BadRequestObjectResult;
            Assert.That(badRequestResult, Is.Not.Null);

            var apiResponse = badRequestResult.Value as ApiResponse<GetProductByNameQuery>;
            Assert.That(apiResponse, Is.Not.Null);
            Assert.That(apiResponse.IsSuccess, Is.False);
            Assert.That(apiResponse.Message, Is.EqualTo("Invalid request"));
            Assert.That(apiResponse.Errors, Is.Not.Null); // Ensure ModelState errors are captured
        }

        [Test]
        public async Task GetProductByName_ReturnsApiResponseWithError_WhenModelIsNull()
        {
            // Act
            var result = await _controller.CreateProduct(null!);

            // Assert
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
            var badRequestResult = result as BadRequestObjectResult;
            Assert.That(badRequestResult, Is.Not.Null);

            var apiResponse = badRequestResult.Value as ApiResponse<ProductResponse>;
            Assert.That(apiResponse, Is.Not.Null);
            Assert.That(apiResponse.IsSuccess, Is.False);
            Assert.That(apiResponse.Message, Is.EqualTo("Request cannot be null!"));
        }

        protected static CreateProductCommand CreateValidCreateProductCommand()
        {
            return new CreateProductCommand(
                Name: "Product Name",
                Summary: "Product Summary",
                Description: "Product Description",
                ImageFile: "Product Picture",
                Price: 100m,
                Quantity: 10,
                BrandId: Guid.NewGuid(),
                CategoryId: Guid.NewGuid()
            );
        }

        protected static UpdateProductCommand CreateValidUpdateProductCommand()
        {
            return new UpdateProductCommand(
                Id: Guid.NewGuid(),
                Name: "Product Name",
                Summary: "Product Summary",
                Description: "Product Description",
                ImageFile: "Product Picture",
                Price: 100m,
                Quantity: 10,
                BrandId: Guid.NewGuid(),
                CategoryId: Guid.NewGuid()
            );
        }
    }
}
