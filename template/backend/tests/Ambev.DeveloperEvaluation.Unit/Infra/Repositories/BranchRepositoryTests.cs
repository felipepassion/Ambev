using Ambev.DeveloperEvaluation.ORM;
using Ambev.DeveloperEvaluation.ORM.Repositories;
using Ambev.DeveloperEvaluation.Unit.Infra.Repositories.TestData;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Infra.Repositories;

public class BranchRepositoryTests
{
    private BranchRepository SetupInMemoryDatabase()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseInMemoryDatabase(databaseName: $"{Guid.NewGuid()}-Db")
            .Options;
        var context = new DefaultContext(options);
        return new BranchRepository(context);
    }

    [Fact]
    public async Task CreateAsync_ShouldAddBranchToDatabase()
    {
        // arrange
        var repository = SetupInMemoryDatabase();
        var branch = BranchRepositoryTestData.CreateBranch("Main Branch");

        // act
        var created = await repository.CreateAsync(branch);

        // assert
        created.Should().NotBeNull();
        created.Id.Should().NotBe(Guid.Empty);
        var fromDb = await repository.GetByIdAsync(created.Id);
        fromDb.Should().NotBeNull();
        fromDb!.Name.Should().Be("Main Branch");
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenBranchDoesNotExist()
    {
        // arrange
        var repository = SetupInMemoryDatabase();

        // act
        var result = await repository.GetByIdAsync(Guid.NewGuid());

        // assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByNameAsync_ShouldReturnCorrectBranch()
    {
        // arrange
        var repository = SetupInMemoryDatabase();
        var branch = BranchRepositoryTestData.CreateBranch("Unique Branch");
        await repository.CreateAsync(branch);

        // act
        var retrieved = await repository.GetByNameAsync("Unique Branch");

        // assert
        retrieved.Should().NotBeNull();
        retrieved!.Name.Should().Be("Unique Branch");
    }

    [Fact]
    public async Task GetAllPagedAsync_ShouldReturnCorrectPage()
    {
        // arrange
        var repository = SetupInMemoryDatabase();
        for (int i = 0; i < 5; i++)
        {
            await repository.CreateAsync(BranchRepositoryTestData.CreateBranch($"Branch_{i}"));
        }

        // act: Request the second page with a page size of 2
        var (branches, totalCount) = await repository.GetAllPagedAsync(2, 2);

        // assert
        totalCount.Should().Be(5);      // Total number of branches
        branches.Should().HaveCount(2); // Page size = 2 (second page)
        branches.First().Name.Should().Be("Branch_2");
        branches.Last().Name.Should().Be("Branch_3");
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveBranchFromDatabase()
    {
        // arrange
        var repository = SetupInMemoryDatabase();
        var branch = BranchRepositoryTestData.CreateBranch("Deletable Branch");
        var created = await repository.CreateAsync(branch);

        // act
        var success = await repository.DeleteAsync(created.Id);

        // assert
        success.Should().BeTrue();
        var fromDb = await repository.GetByIdAsync(created.Id);
        fromDb.Should().BeNull();
    }
}
