using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.ORM;
using Ambev.DeveloperEvaluation.ORM.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Infra.Repositories
{
    public class SaleItemRepositoryTests
    {
        private readonly ISaleItemRepository _saleItemRepository;
        private readonly DefaultContext _context;

        public SaleItemRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<DefaultContext>()
                .UseInMemoryDatabase(databaseName: $"{nameof(SaleItemRepositoryTests)}Db")
                .Options;

            _context = new DefaultContext(options);
            _saleItemRepository = new SaleItemRepository(_context);
        }

        [Fact]
        public async Task CreateAsync_ShouldAddSaleItemToDatabase()
        {
            var saleItem = new SaleItem
            {
                SaleId = Guid.NewGuid(),
                ProductId = Guid.NewGuid(),
                Quantity = 5,
                UnitPrice = 10m,
                Discount = 0.1m,
                TotalItemAmount = 45m
            };

            var created = await _saleItemRepository.CreateAsync(saleItem);

            created.Should().NotBeNull();
            created.Id.Should().NotBe(Guid.Empty);

            var fromDb = await _context.SaleItems.FindAsync(created.Id);
            fromDb.Should().NotBeNull();
            fromDb!.Quantity.Should().Be(5);
            fromDb.Discount.Should().Be(0.1m);
            fromDb.TotalItemAmount.Should().Be(45m);
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateSaleItemInDatabase()
        {
            var saleItem = new SaleItem
            {
                SaleId = Guid.NewGuid(),
                ProductId = Guid.NewGuid(),
                Quantity = 2,
                UnitPrice = 15m,
                Discount = 0m,
                TotalItemAmount = 30m
            };

            var created = await _saleItemRepository.CreateAsync(saleItem);

            created.Quantity = 3;
            created.Discount = 0.2m;
            created.TotalItemAmount = 36m;

            var updated = await _saleItemRepository.UpdateAsync(created);

            updated.Quantity.Should().Be(3);
            updated.Discount.Should().Be(0.2m);
            updated.TotalItemAmount.Should().Be(36m);

            var fromDb = await _context.SaleItems.FindAsync(created.Id);
            fromDb.Should().NotBeNull();
            fromDb!.Quantity.Should().Be(3);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnCorrectSaleItem()
        {
            var saleItem = new SaleItem
            {
                SaleId = Guid.NewGuid(),
                ProductId = Guid.NewGuid(),
                Quantity = 1,
                UnitPrice = 100m,
                Discount = 0m,
                TotalItemAmount = 100m
            };

            var created = await _saleItemRepository.CreateAsync(saleItem);

            var fetched = await _saleItemRepository.GetByIdAsync(created.Id);
            fetched.Should().NotBeNull();
            fetched!.UnitPrice.Should().Be(100m);
            fetched.Quantity.Should().Be(1);
        }

        [Fact]
        public async Task GetBySaleIdAsync_ShouldReturnAllItemsForASale()
        {
            var saleId = Guid.NewGuid();

            for (int i = 1; i <= 3; i++)
            {
                await _saleItemRepository.CreateAsync(new SaleItem
                {
                    SaleId = saleId,
                    ProductId = Guid.NewGuid(),
                    Quantity = i,
                    UnitPrice = 10m,
                    Discount = 0m,
                    TotalItemAmount = 10m * i
                });
            }

            var items = await _saleItemRepository.GetBySaleIdAsync(saleId);
            items.Should().HaveCount(3);
            items.Select(x => x.Quantity).Should().BeEquivalentTo(new[] { 1, 2, 3 });
        }

        [Fact]
        public async Task DeleteAsync_ShouldRemoveSaleItemFromDatabase()
        {
            var saleItem = new SaleItem
            {
                SaleId = Guid.NewGuid(),
                ProductId = Guid.NewGuid(),
                Quantity = 2,
                UnitPrice = 20m,
                TotalItemAmount = 40m
            };

            var created = await _saleItemRepository.CreateAsync(saleItem);

            var success = await _saleItemRepository.DeleteAsync(created.Id);
            success.Should().BeTrue();

            var fromDb = await _context.SaleItems.FindAsync(created.Id);
            fromDb.Should().BeNull();
        }
    }
}
