using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
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

        var _context = new DefaultContext(options);
        return new SaleRepository(_context);
    }

    [Fact]
    public async Task CreateAsync_ShouldAddSaleToDatabase()
    {
        var _saleRepository = SetupDatabaseContext();

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

        var created = await _saleRepository.CreateAsync(sale);

        created.Should().NotBeNull();
        created.Id.Should().NotBe(Guid.Empty);

        var fromDb = await _saleRepository.GetByIdAsync(created.Id);
        fromDb.Should().NotBeNull();
        fromDb!.SaleNumber.Should().Be("SALE-123");
        fromDb.TotalAmount.Should().Be(sale.TotalAmount);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateSaleInDatabase()
    {
        var _saleRepository = SetupDatabaseContext();

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
        var created = await _saleRepository.CreateAsync(sale);

        created.SaleNumber = "SALE-XYZ";
        created.IsCancelled = true;

        var updated = await _saleRepository.UpdateAsync(created);

        updated.SaleNumber.Should().Be("SALE-XYZ");
        updated.IsCancelled.Should().BeTrue();
        updated.TotalAmount.Should().Be(sale.TotalAmount);

        var fromDb = await _saleRepository.GetByIdAsync(created.Id);
        fromDb.Should().NotBeNull();
        fromDb!.SaleNumber.Should().Be("SALE-XYZ");
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenSaleDoesNotExist()
    {
        var _saleRepository = SetupDatabaseContext();

        var result = await _saleRepository.GetByIdAsync(Guid.NewGuid());
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetBySaleNumberAsync_ShouldReturnCorrectSale()
    {
        var _saleRepository = SetupDatabaseContext();

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
        await _saleRepository.CreateAsync(sale);

        var foundSale = await _saleRepository.GetBySaleNumberAsync("SALE-FIND");
        foundSale.Should().NotBeNull();
        foundSale!.SaleNumber.Should().Be("SALE-FIND");
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveSaleFromDatabase()
    {
        var _saleRepository = SetupDatabaseContext();

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
        var created = await _saleRepository.CreateAsync(sale);

        var success = await _saleRepository.DeleteAsync(created.Id);
        success.Should().BeTrue();

        var fromDb = await _saleRepository.GetByIdAsync(created.Id);
        fromDb.Should().BeNull();
    }
}
