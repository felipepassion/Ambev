using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.ORM;
using Ambev.DeveloperEvaluation.ORM.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Infra.Repositories;

public class SaleRepositoryTests
{
    private SaleRepository SetupDatabaseContext()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseInMemoryDatabase(databaseName: $"{nameof(SaleRepositoryTests)}Db")
            .Options;
        var context = new DefaultContext(options);
        return new SaleRepository(context);
    }

    [Fact]
    public async Task CreateAsync_ShouldAddSaleToDatabase()
    {
        // arrange
        var saleRepository = SetupDatabaseContext();
        var sale = new Sale
        {
            SaleNumber = "SALE-123",
            SaleDate = DateTime.UtcNow,
            UserId = Guid.NewGuid(),
            BranchId = Guid.NewGuid(),
            Items = new List<SaleItem>
            {
                new SaleItem
                {
                    ProductId = Guid.NewGuid(),
                    Quantity = 2,
                    UnitPrice = 50.25m
                }
            }
        };

        // act
        var created = await saleRepository.CreateAsync(sale);

        // assert
        created.Should().NotBeNull();
        created.Id.Should().NotBe(Guid.Empty);
        var fromDb = await saleRepository.GetByIdAsync(created.Id);
        fromDb.Should().NotBeNull();
        fromDb!.SaleNumber.Should().Be("SALE-123");
        fromDb.TotalAmount.Should().Be(sale.TotalAmount);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateSaleInDatabase()
    {
        // arrange
        var saleRepository = SetupDatabaseContext();
        var sale = new Sale
        {
            SaleNumber = "SALE-ABC",
            SaleDate = DateTime.UtcNow,
            UserId = Guid.NewGuid(),
            BranchId = Guid.NewGuid(),
            Items = new List<SaleItem>
            {
                new SaleItem
                {
                    ProductId = Guid.NewGuid(),
                    Quantity = 2,
                    UnitPrice = 50.25m
                }
            }
        };
        var created = await saleRepository.CreateAsync(sale);

        // act
        created.SaleNumber = "SALE-XYZ";
        created.IsCancelled = true;
        var updated = await saleRepository.UpdateAsync(created);

        // assert
        updated.SaleNumber.Should().Be("SALE-XYZ");
        updated.IsCancelled.Should().BeTrue();
        updated.TotalAmount.Should().Be(sale.TotalAmount);
        var fromDb = await saleRepository.GetByIdAsync(created.Id);
        fromDb.Should().NotBeNull();
        fromDb!.SaleNumber.Should().Be("SALE-XYZ");
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenSaleDoesNotExist()
    {
        // arrange
        var saleRepository = SetupDatabaseContext();

        // act
        var result = await saleRepository.GetByIdAsync(Guid.NewGuid());

        // assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetBySaleNumberAsync_ShouldReturnCorrectSale()
    {
        // arrange
        var saleRepository = SetupDatabaseContext();
        var sale = new Sale
        {
            SaleNumber = "SALE-FIND",
            SaleDate = DateTime.UtcNow,
            UserId = Guid.NewGuid(),
            BranchId = Guid.NewGuid(),
            Items = new List<SaleItem>
            {
                new SaleItem
                {
                    ProductId = Guid.NewGuid(),
                    Quantity = 2,
                    UnitPrice = 50.25m
                }
            }
        };
        await saleRepository.CreateAsync(sale);

        // act
        var foundSale = await saleRepository.GetBySaleNumberAsync("SALE-FIND");

        // assert
        foundSale.Should().NotBeNull();
        foundSale!.SaleNumber.Should().Be("SALE-FIND");
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveSaleFromDatabase()
    {
        // arrange
        var saleRepository = SetupDatabaseContext();
        var sale = new Sale
        {
            SaleNumber = "SALE-DELETE",
            SaleDate = DateTime.UtcNow,
            UserId = Guid.NewGuid(),
            BranchId = Guid.NewGuid(),
            Items = new List<SaleItem>
            {
                new SaleItem
                {
                    ProductId = Guid.NewGuid(),
                    Quantity = 2,
                    UnitPrice = 50.25m
                }
            }
        };
        var created = await saleRepository.CreateAsync(sale);

        // act
        var success = await saleRepository.DeleteAsync(created.Id);

        // assert
        success.Should().BeTrue();
        var fromDb = await saleRepository.GetByIdAsync(created.Id);
        fromDb.Should().BeNull();
    }

    [Fact]
    public async Task CancelAsync_ShouldRemoveSaleFromDatabase()
    {
        // arrange
        var saleRepository = SetupDatabaseContext();
        var sale = new Sale
        {
            SaleNumber = "SALE-CANCEL",
            SaleDate = DateTime.UtcNow,
            UserId = Guid.NewGuid(),
            BranchId = Guid.NewGuid(),
            Items = new List<SaleItem>
            {
                new SaleItem
                {
                    ProductId = Guid.NewGuid(),
                    Quantity = 2,
                    UnitPrice = 50.25m
                }
            }
        };
        var created = await saleRepository.CreateAsync(sale);

        // act
        var success = await saleRepository.CancelAsync(created.Id);

        // assert
        success.Should().BeTrue();
        var fromDb = await saleRepository.GetByIdAsync(created.Id);
        fromDb!.IsCancelled.Should().BeTrue();
        fromDb.Should().NotBeNull();
    }
}
