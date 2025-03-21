using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.ORM;
using Ambev.DeveloperEvaluation.ORM.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Ambev.DeveloperEvaluation.Tests.RepositoryTests;

public class SaleRepositoryTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly DefaultContext _context;

    public SaleRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseInMemoryDatabase(databaseName: $"{nameof(SaleRepositoryTests)}Db")
            .Options;

        _context = new DefaultContext(options);
        _saleRepository = new SaleRepository(_context);
    }

    [Fact]
    public async Task CreateAsync_ShouldAddSaleToDatabase()
    {
        var sale = new Sale
        {
            SaleNumber = "SALE-123",
            SaleDate = DateTime.UtcNow,
            UserId = Guid.NewGuid(),
            BranchId = Guid.NewGuid(),
            TotalAmount = 150.75m
        };

        var created = await _saleRepository.CreateAsync(sale);

        created.Should().NotBeNull();
        created.Id.Should().NotBe(Guid.Empty);

        var fromDb = await _context.Sales.FindAsync(created.Id);
        fromDb.Should().NotBeNull();
        fromDb!.SaleNumber.Should().Be("SALE-123");
        fromDb.TotalAmount.Should().Be(150.75m);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateSaleInDatabase()
    {
        var sale = new Sale
        {
            SaleNumber = "SALE-ABC",
            SaleDate = DateTime.UtcNow,
            UserId = Guid.NewGuid(),
            BranchId = Guid.NewGuid(),
            TotalAmount = 99.99m
        };
        var created = await _saleRepository.CreateAsync(sale);

        created.SaleNumber = "SALE-XYZ";
        created.IsCancelled = true;
        created.TotalAmount = 120.00m;

        var updated = await _saleRepository.UpdateAsync(created);

        updated.SaleNumber.Should().Be("SALE-XYZ");
        updated.IsCancelled.Should().BeTrue();
        updated.TotalAmount.Should().Be(120.00m);

        var fromDb = await _context.Sales.FindAsync(created.Id);
        fromDb.Should().NotBeNull();
        fromDb!.SaleNumber.Should().Be("SALE-XYZ");
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenSaleDoesNotExist()
    {
        var result = await _saleRepository.GetByIdAsync(Guid.NewGuid());
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetBySaleNumberAsync_ShouldReturnCorrectSale()
    {
        var sale = new Sale
        {
            SaleNumber = "SALE-FIND",
            SaleDate = DateTime.UtcNow,
            UserId = Guid.NewGuid(),
            BranchId = Guid.NewGuid(),
            TotalAmount = 250m
        };
        await _saleRepository.CreateAsync(sale);

        var foundSale = await _saleRepository.GetBySaleNumberAsync("SALE-FIND");
        foundSale.Should().NotBeNull();
        foundSale!.SaleNumber.Should().Be("SALE-FIND");
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveSaleFromDatabase()
    {
        var sale = new Sale
        {
            SaleNumber = "SALE-DELETE",
            SaleDate = DateTime.UtcNow,
            UserId = Guid.NewGuid(),
            BranchId = Guid.NewGuid(),
            TotalAmount = 999.99m
        };
        var created = await _saleRepository.CreateAsync(sale);

        var success = await _saleRepository.DeleteAsync(created.Id);
        success.Should().BeTrue();

        var fromDb = await _context.Sales.FindAsync(created.Id);
        fromDb.Should().BeNull();
    }
}
