using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.ORM;
using Ambev.DeveloperEvaluation.ORM.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Infra.Repositories;

public class SaleItemRepositoryTests
{
    private SaleItemRepository SetupDatabaseContext()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseInMemoryDatabase(databaseName: $"{Guid.NewGuid()}-Db")
            .Options;
        var context = new DefaultContext(options);
        return new SaleItemRepository(context);
    }

    [Fact]
    public async Task CreateAsync_ShouldAddSaleItemToDatabase()
    {
        // arrange
        var saleItemRepository = SetupDatabaseContext();
        var saleItem = new SaleItem
        {
            SaleId = Guid.NewGuid(),
            ProductId = Guid.NewGuid(),
            Quantity = 5,
            UnitPrice = 10m,
            Discount = 0.1m,
            TotalItemAmount = 45m
        };

        // act
        var created = await saleItemRepository.CreateAsync(saleItem);

        // assert
        created.Should().NotBeNull();
        created.Id.Should().NotBe(Guid.Empty);
        var fromDb = await saleItemRepository.GetByIdAsync(created.Id);
        fromDb.Should().NotBeNull();
        fromDb!.Quantity.Should().Be(5);
        fromDb.Discount.Should().Be(0.1m);
        fromDb.TotalItemAmount.Should().Be(45m);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateSaleItemInDatabase()
    {
        // arrange
        var saleItemRepository = SetupDatabaseContext();
        var saleItem = new SaleItem
        {
            SaleId = Guid.NewGuid(),
            ProductId = Guid.NewGuid(),
            Quantity = 2,
            UnitPrice = 15m,
            Discount = 0m,
            TotalItemAmount = 30m
        };
        var created = await saleItemRepository.CreateAsync(saleItem);

        // act
        created.Quantity = 3;
        created.Discount = 0.2m;
        created.TotalItemAmount = 36m;
        var updated = await saleItemRepository.UpdateAsync(created);

        // assert
        updated.Quantity.Should().Be(3);
        updated.Discount.Should().Be(0.2m);
        updated.TotalItemAmount.Should().Be(36m);
        var fromDb = await saleItemRepository.GetByIdAsync(created.Id);
        fromDb.Should().NotBeNull();
        fromDb!.Quantity.Should().Be(3);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnCorrectSaleItem()
    {
        // arrange
        var saleItemRepository = SetupDatabaseContext();
        var saleItem = new SaleItem
        {
            SaleId = Guid.NewGuid(),
            ProductId = Guid.NewGuid(),
            Quantity = 1,
            UnitPrice = 100m,
            Discount = 0m,
            TotalItemAmount = 100m
        };
        var created = await saleItemRepository.CreateAsync(saleItem);

        // act
        var fetched = await saleItemRepository.GetByIdAsync(created.Id);

        // assert
        fetched.Should().NotBeNull();
        fetched!.UnitPrice.Should().Be(100m);
        fetched.Quantity.Should().Be(1);
    }

    [Fact]
    public async Task GetBySaleIdAsync_ShouldReturnAllItemsForASale()
    {
        // arrange
        var saleItemRepository = SetupDatabaseContext();
        var saleId = Guid.NewGuid();
        for (int i = 1; i <= 3; i++)
        {
            await saleItemRepository.CreateAsync(new SaleItem
            {
                SaleId = saleId,
                ProductId = Guid.NewGuid(),
                Quantity = i,
                UnitPrice = 10m,
                Discount = 0m,
                TotalItemAmount = 10m * i
            });
        }

        // act
        var items = await saleItemRepository.GetBySaleIdAsync(saleId);

        // assert
        items.Should().HaveCount(3);
        items.Select(x => x.Quantity).Should().BeEquivalentTo(new[] { 1, 2, 3 });
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveSaleItemFromDatabase()
    {
        // arrange
        var saleItemRepository = SetupDatabaseContext();
        var saleItem = new SaleItem
        {
            SaleId = Guid.NewGuid(),
            ProductId = Guid.NewGuid(),
            Quantity = 2,
            UnitPrice = 20m,
            TotalItemAmount = 40m
        };
        var created = await saleItemRepository.CreateAsync(saleItem);

        // act
        var success = await saleItemRepository.DeleteAsync(created.Id);

        // assert
        success.Should().BeTrue();
        var fromDb = await saleItemRepository.GetByIdAsync(created.Id);
        fromDb.Should().BeNull();
    }
}
