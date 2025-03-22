using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.ORM;
using Ambev.DeveloperEvaluation.ORM.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Infra.Repositories
{
    public class ProductRepositoryTests
    {
        private readonly IProductRepository _productRepository;
        private readonly DefaultContext _context;

        public ProductRepositoryTests()
        {
            // Cria um DbContext InMemory para os testes
            var options = new DbContextOptionsBuilder<DefaultContext>()
                .UseInMemoryDatabase(databaseName: $"{nameof(ProductRepositoryTests)}Db")
                .Options;

            _context = new DefaultContext(options);
            _productRepository = new ProductRepository(_context);
        }

        [Fact]
        public async Task CreateAsync_ShouldAddProductToDatabase()
        {
            var product = new Product
            {
                Name = "Test Product",
                Description = "Any description",
                UnitPrice = 12.99m
            };

            var created = await _productRepository.CreateAsync(product);

            created.Should().NotBeNull();
            created.Id.Should().NotBe(Guid.Empty);

            var fromDb = await _context.Products.FindAsync(created.Id);
            fromDb.Should().NotBeNull();
            fromDb!.Name.Should().Be("Test Product");
            fromDb.UnitPrice.Should().Be(12.99m);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNull_WhenProductDoesNotExist()
        {
            var result = await _productRepository.GetByIdAsync(Guid.NewGuid());
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetByNameAsync_ShouldReturnCorrectProduct()
        {
            var product = new Product
            {
                Name = "Special Product",
                Description = "Desc",
                UnitPrice = 29.99m
            };
            await _productRepository.CreateAsync(product);

            var retrieved = await _productRepository.GetByNameAsync("Special Product");
            retrieved.Should().NotBeNull();
            retrieved!.Name.Should().Be("Special Product");
        }

        [Fact]
        public async Task GetAllPagedAsync_ShouldReturnCorrectPage()
        {
            // Insere múltiplos produtos para paginar
            for (int i = 0; i < 5; i++)
            {
                await _productRepository.CreateAsync(new Product
                {
                    Name = $"Product_{i}",
                    UnitPrice = i + 1
                });
            }

            var (products, totalCount) = await _productRepository.GetAllPagedAsync(2, 2);

            totalCount.Should().Be(5);      // Total de produtos
            products.Should().HaveCount(2); // Tamanho da página = 2 (segunda página)
            products.First().Name.Should().Be("Product_2");
            products.Last().Name.Should().Be("Product_3");
        }

        [Fact]
        public async Task DeleteAsync_ShouldRemoveProductFromDatabase()
        {
            var product = new Product
            {
                Name = "Deletable Product",
                UnitPrice = 45.50m
            };
            var created = await _productRepository.CreateAsync(product);

            var success = await _productRepository.DeleteAsync(created.Id);
            success.Should().BeTrue();

            var fromDb = await _context.Products.FindAsync(created.Id);
            fromDb.Should().BeNull();
        }
    }
}
