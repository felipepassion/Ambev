using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.ORM;
using Ambev.DeveloperEvaluation.ORM.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Infra.Repositories;

public class UserRepositoryTests
{
    private UserRepository CreateScopedRepository()
    {
        var randomId = Guid.NewGuid().ToString().Split("-")[0];
        var options = new DbContextOptionsBuilder<DefaultContext>()
                    .UseInMemoryDatabase(databaseName: $"{nameof(UserRepositoryTests)}-{randomId}-Db")
                    .Options;
        var context = new DefaultContext(options);
        return new UserRepository(context);
    }

    [Fact]
    public async Task CreateAsync_ShouldAddUserToDatabase()
    {
        // arrange
        var userRepository = CreateScopedRepository();
        var user = new User
        {
            Username = "John Doe",
            Email = "john.doe@example.com",
            Phone = "+5511999999999",
            Password = "Abc@12345",
            Role = UserRole.Admin,
            Status = UserStatus.Active
        };

        // act
        var created = await userRepository.CreateAsync(user);

        // assert
        created.Should().NotBeNull();
        created.Id.Should().NotBe(Guid.Empty);
        var fromDb = await userRepository.GetByIdAsync(created.Id);
        fromDb.Should().NotBeNull();
        fromDb!.Email.Should().Be("john.doe@example.com");
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_IfUserDoesNotExist()
    {
        // arrange
        var userRepository = CreateScopedRepository();

        // act
        var result = await userRepository.GetByIdAsync(Guid.NewGuid());

        // assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByEmailAsync_ShouldReturnCorrectUser()
    {
        // arrange
        var userRepository = CreateScopedRepository();
        var user = new User
        {
            Username = "Jane Doe",
            Email = "jane.doe@example.com",
            Phone = "+5511888888888",
            Password = "Abc@12345",
            Role = UserRole.Customer,
            Status = UserStatus.Active
        };
        await userRepository.CreateAsync(user);

        // act
        var fetched = await userRepository.GetByEmailAsync("jane.doe@example.com");

        // assert
        fetched.Should().NotBeNull();
        fetched!.Username.Should().Be("Jane Doe");
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveUserFromDatabase()
    {
        // arrange
        var userRepository = CreateScopedRepository();
        var user = new User
        {
            Username = "Delete",
            Email = "delete@example.com",
            Phone = "+5511222333444",
            Password = "Abc@12345",
            Role = UserRole.Customer,
            Status = UserStatus.Active
        };
        var created = await userRepository.CreateAsync(user);

        // act
        var success = await userRepository.DeleteAsync(created.Id);

        // assert
        success.Should().BeTrue();
        var fromDb = await userRepository.GetByIdAsync(created.Id);
        fromDb.Should().BeNull();
    }

    [Fact(DisplayName = "GetAllUsersAsync should return correct users for the given page")]
    public async Task GetAllUsersAsync_ShouldReturnCorrectUsersForPage()
    {
        // arrange: Insert multiple users into the in-memory database
        var userRepository = CreateScopedRepository();
        var users = new List<User>
        {
            new User { Username = "Alice", Email = "alice@example.com", Phone = "+551100000000", Password = "pwd", Role = UserRole.Customer, Status = UserStatus.Active },
            new User { Username = "Bob", Email = "bob@example.com", Phone = "+551100000001", Password = "pwd", Role = UserRole.Customer, Status = UserStatus.Active },
            new User { Username = "Charlie", Email = "charlie@example.com", Phone = "+551100000002", Password = "pwd", Role = UserRole.Customer, Status = UserStatus.Active },
            new User { Username = "David", Email = "david@example.com", Phone = "+551100000003", Password = "pwd", Role = UserRole.Customer, Status = UserStatus.Active },
            new User { Username = "Eve", Email = "eve@example.com", Phone = "+551100000004", Password = "pwd", Role = UserRole.Customer, Status = UserStatus.Active },
            new User { Username = "Frank", Email = "frank@example.com", Phone = "+551100000005", Password = "pwd", Role = UserRole.Customer, Status = UserStatus.Active },
        };

        foreach (var user in users)
            await userRepository.CreateAsync(user);

        // act: Request page 1 with a page size of 3 and page 2 with a page size of 3
        var page1 = await userRepository.GetAllUsersAsync(1, 3);
        var page2 = await userRepository.GetAllUsersAsync(2, 3);

        // assert: Page 1 should contain "Alice", "Bob", "Charlie" (ordered by Username)
        page1.Should().HaveCount(3);
        page1[0].Username.Should().Be("Alice");
        page1[1].Username.Should().Be("Bob");
        page1[2].Username.Should().Be("Charlie");

        // assert: Page 2 should contain "David", "Eve", "Frank"
        page2.Should().HaveCount(3);
        page2[0].Username.Should().Be("David");
        page2[1].Username.Should().Be("Eve");
        page2[2].Username.Should().Be("Frank");
    }

    [Fact(DisplayName = "GetAllUsersAsync should return an empty list if page is out of range")]
    public async Task GetAllUsersAsync_ShouldReturnEmptyList_ForPageOutOfRange()
    {
        // arrange: Insert only 2 users into the in-memory database
        var userRepository = CreateScopedRepository();
        var users = new List<User>
        {
            new User { Username = "Alice", Email = "alice@example.com", Phone = "+551100000000", Password = "pwd", Role = UserRole.Customer, Status = UserStatus.Active },
            new User { Username = "Bob", Email = "bob@example.com", Phone = "+551100000001", Password = "pwd", Role = UserRole.Customer, Status = UserStatus.Active },
        };

        foreach (var user in users)
            await userRepository.CreateAsync(user);

        // act: Request page 2 with a page size of 2 (out of range)
        var page2 = await userRepository.GetAllUsersAsync(2, 2);

        // assert: The returned list should be empty
        page2.Should().BeEmpty();
    }
}
