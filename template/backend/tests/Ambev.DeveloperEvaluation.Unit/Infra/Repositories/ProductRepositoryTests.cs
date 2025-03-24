using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.ORM;
using Ambev.DeveloperEvaluation.ORM.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Infra.Repositories;

public class ProductRepositoryTests
{
    public ProductRepository SetupInMemoryDatabase()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseInMemoryDatabase(databaseName: $"{Guid.NewGuid()}-Db")
            .Options;
        var context = new DefaultContext(options);
        return new ProductRepository(context);
    }

    [Fact]
    public async Task CreateAsync_ShouldAddProductToDatabase()
    {
        // arrange
        var productRepository = SetupInMemoryDatabase();
        var product = new Product
        {
            Name = "Test Product",
            Description = "Any description",
            UnitPrice = 12.99m
        };

        // act
        var created = await productRepository.CreateAsync(product);

        // assert
        created.Should().NotBeNull();
        created.Id.Should().NotBe(Guid.Empty);
        var fromDb = await productRepository.GetByIdAsync(created.Id);
        fromDb.Should().NotBeNull();
        fromDb!.Name.Should().Be("Test Product");
        fromDb.UnitPrice.Should().Be(12.99m);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenProductDoesNotExist()
    {
        // arrange
        var productRepository = SetupInMemoryDatabase();

        // act
        var result = await productRepository.GetByIdAsync(Guid.NewGuid());

        // assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByNameAsync_ShouldReturnCorrectProduct()
    {
        // arrange
        var productRepository = SetupInMemoryDatabase();
        var product = new Product
        {
            Name = "Special Product",
            Description = "Desc",
            UnitPrice = 29.99m
        };
        await productRepository.CreateAsync(product);

        // act
        var retrieved = await productRepository.GetByNameAsync("Special Product");

        // assert
        retrieved.Should().NotBeNull();
        retrieved!.Name.Should().Be("Special Product");
    }

    [Fact]
    public async Task GetAllPagedAsync_ShouldReturnCorrectPage()
    {
        // arrange
        var productRepository = SetupInMemoryDatabase();
        for (int i = 0; i < 5; i++)
        {
            await productRepository.CreateAsync(new Product
            {
                Name = $"Product_{i}",
                UnitPrice = i + 1
            });
        }

        // act
        var (products, totalCount) = await productRepository.GetAllPagedAsync(2, 2);

        // assert
        totalCount.Should().Be(5);      // Total number of products
        products.Should().HaveCount(2); // Page size = 2 (second page)
        products.First().Name.Should().Be("Product_2");
        products.Last().Name.Should().Be("Product_3");
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveProductFromDatabase()
    {
        // arrange
        var productRepository = SetupInMemoryDatabase();
        var product = new Product
        {
            Name = "Deletable Product",
            UnitPrice = 45.50m
        };
        var created = await productRepository.CreateAsync(product);

        // act
        var success = await productRepository.DeleteAsync(created.Id);

        // assert
        success.Should().BeTrue();
        var fromDb = await productRepository.GetByIdAsync(created.Id);
        fromDb.Should().BeNull();
    }
}
