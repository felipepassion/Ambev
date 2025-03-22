using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.ORM;
using Ambev.DeveloperEvaluation.ORM.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Infra.Repositories;

public class UserRepositoryTests
{
    private readonly IUserRepository _userRepository;
    private readonly DefaultContext _context;

    public UserRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseInMemoryDatabase(databaseName: $"{nameof(UserRepositoryTests)}Db")
            .Options;

        _context = new DefaultContext(options);
        _userRepository = new UserRepository(_context);
    }

    [Fact]
    public async Task CreateAsync_ShouldAddUserToDatabase()
    {
        var user = new User
        {
            Username = "John Doe",
            Email = "john.doe@example.com",
            Phone = "+5511999999999",
            Password = "Abc@12345",
            Role = UserRole.Admin,
            Status = UserStatus.Active
        };

        var created = await _userRepository.CreateAsync(user);

        created.Should().NotBeNull();
        created.Id.Should().NotBe(Guid.Empty);

        var fromDb = await _context.Users.FindAsync(created.Id);
        fromDb.Should().NotBeNull();
        fromDb!.Email.Should().Be("john.doe@example.com");
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_IfUserDoesNotExist()
    {
        var result = await _userRepository.GetByIdAsync(Guid.NewGuid());
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByEmailAsync_ShouldReturnCorrectUser()
    {
        var user = new User
        {
            Username = "Jane Doe",
            Email = "jane.doe@example.com",
            Phone = "+5511888888888",
            Password = "Abc@12345",
            Role = UserRole.Customer,
            Status = UserStatus.Active
        };
        await _userRepository.CreateAsync(user);

        var fetched = await _userRepository.GetByEmailAsync("jane.doe@example.com");
        fetched.Should().NotBeNull();
        fetched!.Username.Should().Be("Jane Doe");
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveUserFromDatabase()
    {
        var user = new User
        {
            Username = "Delete",
            Email = "delete@example.com",
            Phone = "+5511222333444",
            Password = "Abc@12345",
            Role = UserRole.Customer,
            Status = UserStatus.Active
        };
        var created = await _userRepository.CreateAsync(user);

        var success = await _userRepository.DeleteAsync(created.Id);
        success.Should().BeTrue();

        var fromDb = await _context.Users.FindAsync(created.Id);
        fromDb.Should().BeNull();
    }
}
