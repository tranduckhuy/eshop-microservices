using Catalog.Domain.Entities;
using Catalog.Infrastructure.Data;
using Catalog.Infrastructure.Repositories;
using MongoDB.Driver;
using Moq;

namespace Catalog.Infrastructure.Test
{
    [TestFixture]
    public class UnitCategoryRepositoryTest
    {
        private Mock<ICatalogContext> _mockContext;
        private Mock<IMongoCollection<Category>> _mockCollection;
        private Mock<IAsyncCursor<Category>> _mockCursor;
        private CategoryRepository _repository;
        private List<Category> _categories;

        [SetUp]
        public void SetUp()
        {
            _mockContext = new Mock<ICatalogContext>();
            _mockCollection = new Mock<IMongoCollection<Category>>();
            _mockCursor = new Mock<IAsyncCursor<Category>>();
            _categories = new List<Category>
            {
                new Category { Id = Guid.NewGuid(), Name = "Category1" },
                new Category { Id = Guid.NewGuid(), Name = "Category2" }
            };

            _mockCursor.SetupSequence(x => x.MoveNext(It.IsAny<CancellationToken>())).Returns(true).Returns(false);
            _mockCursor.SetupSequence(x => x.MoveNextAsync(It.IsAny<CancellationToken>())).ReturnsAsync(true).ReturnsAsync(false);
            _mockCursor.Setup(x => x.Current).Returns(_categories);

            _mockCollection.Setup(x => x.FindAsync(It.IsAny<FilterDefinition<Category>>(), It.IsAny<FindOptions<Category>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(_mockCursor.Object);

            _mockContext.Setup(c => c.Categories).Returns(_mockCollection.Object);

            _repository = new CategoryRepository(_mockContext.Object);
        }

        [Test]
        public async Task GetAll_ShouldReturnAllCategories()
        {
            var result = await _repository.GetAll();
            Assert.That(result.Count(), Is.EqualTo(2));
            Assert.That(result.First().Name, Is.EqualTo("Category1"));
        }

        [Test]
        public async Task GetById_ShouldReturnCorrectCategory()
        {
            var CategoryId = _categories.First().Id;
            var filter = Builders<Category>.Filter.Eq(b => b.Id, CategoryId);

            _mockCollection.Setup(x => x.FindAsync(It.Is<FilterDefinition<Category>>(f => f == filter), It.IsAny<FindOptions<Category>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(_mockCursor.Object);

            var result = await _repository.GetById(CategoryId);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Name, Is.EqualTo("Category1"));
        }
    }
}