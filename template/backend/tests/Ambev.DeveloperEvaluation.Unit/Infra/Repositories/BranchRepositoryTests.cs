using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.ORM;
using Ambev.DeveloperEvaluation.ORM.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Infra.Repositories
{
    public class BranchRepositoryTests
    {
        private readonly IBranchRepository _branchRepository;
        private readonly DefaultContext _context;

        public BranchRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<DefaultContext>()
                .UseInMemoryDatabase(databaseName: $"{nameof(BranchRepositoryTests)}Db")
                .Options;

            _context = new DefaultContext(options);
            _branchRepository = new BranchRepository(_context);
        }

        [Fact]
        public async Task CreateAsync_ShouldAddBranchToDatabase()
        {
            var branch = new Branch
            {
                Name = "Main Branch"
            };

            var created = await _branchRepository.CreateAsync(branch);

            created.Should().NotBeNull();
            created.Id.Should().NotBe(Guid.Empty);

            var fromDb = await _context.Branches.FindAsync(created.Id);
            fromDb.Should().NotBeNull();
            fromDb!.Name.Should().Be("Main Branch");
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNull_WhenBranchDoesNotExist()
        {
            var result = await _branchRepository.GetByIdAsync(Guid.NewGuid());
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetByNameAsync_ShouldReturnCorrectBranch()
        {
            var branch = new Branch
            {
                Name = "Unique Branch"
            };
            await _branchRepository.CreateAsync(branch);

            var retrieved = await _branchRepository.GetByNameAsync("Unique Branch");
            retrieved.Should().NotBeNull();
            retrieved!.Name.Should().Be("Unique Branch");
        }

        [Fact]
        public async Task GetAllPagedAsync_ShouldReturnCorrectPage()
        {
            // Insert multiple branches to test pagination
            for (int i = 0; i < 5; i++)
            {
                await _branchRepository.CreateAsync(new Branch
                {
                    Name = $"Branch_{i}"
                });
            }

            var (branches, totalCount) = await _branchRepository.GetAllPagedAsync(2, 2);

            totalCount.Should().Be(5);      // Total number of branches
            branches.Should().HaveCount(2); // Page size = 2 (getting the second page)
            branches.First().Name.Should().Be("Branch_2");
            branches.Last().Name.Should().Be("Branch_3");
        }

        [Fact]
        public async Task DeleteAsync_ShouldRemoveBranchFromDatabase()
        {
            var branch = new Branch
            {
                Name = "Deletable Branch"
            };
            var created = await _branchRepository.CreateAsync(branch);

            var success = await _branchRepository.DeleteAsync(created.Id);
            success.Should().BeTrue();

            var fromDb = await _context.Branches.FindAsync(created.Id);
            fromDb.Should().BeNull();
        }
    }
}
