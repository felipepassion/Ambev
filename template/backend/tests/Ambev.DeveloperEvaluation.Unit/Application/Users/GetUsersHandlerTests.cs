using Ambev.DeveloperEvaluation.Application.Users.GetUser;
using Ambev.DeveloperEvaluation.Application.Users.GetUsers;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Tests.Unit.Application.Users;

/// <summary>
/// Contains unit tests for the <see cref="GetUsersHandler"/> class.
/// </summary>
public class GetUsersHandlerTests
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly GetUsersHandler _handler;

    public GetUsersHandlerTests()
    {
        _userRepository = Substitute.For<IUserRepository>();
        _mapper = Substitute.For<IMapper>();
        _handler = new GetUsersHandler(_userRepository, _mapper);
    }

    /// <summary>
    /// Tests that a valid GetUsersCommand returns a list of GetUserResult.
    /// </summary>
    [Fact(DisplayName = "Given valid command When retrieving users Then returns a list of user results")]
    public async Task Handle_ValidCommand_ReturnsListOfUsers()
    {
        // Arrange
        int pageNumber = 1;
        int pageSize = 2;
        var command = new GetUsersCommand(pageNumber, pageSize);

        var user1 = new User
        {
            Id = Guid.NewGuid(),
            Username = "User1",
            Email = "user1@example.com",
            Phone = "123456789"
        };

        var user2 = new User
        {
            Id = Guid.NewGuid(),
            Username = "User2",
            Email = "user2@example.com",
            Phone = "987654321"
        };

        var usersList = new List<User> { user1, user2 };

        _userRepository.GetAllUsersAsync(pageNumber, pageSize, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(usersList));

        var result1 = new GetUserResult
        {
            Id = user1.Id,
            Username = user1.Username,
            Email = user1.Email,
            Phone = user1.Phone
        };

        var result2 = new GetUserResult
        {
            Id = user2.Id,
            Username = user2.Username,
            Email = user2.Email,
            Phone = user2.Phone
        };

        _mapper.Map<GetUserResult>(user1).Returns(result1);
        _mapper.Map<GetUserResult>(user2).Returns(result2);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.First().Id.Should().Be(user1.Id);
        result.Last().Id.Should().Be(user2.Id);

        await _userRepository.Received(1).GetAllUsersAsync(pageNumber, pageSize, Arg.Any<CancellationToken>());
        _mapper.Received(1).Map<GetUserResult>(user1);
        _mapper.Received(1).Map<GetUserResult>(user2);
    }

    /// <summary>
    /// Tests that if no users exist, the handler returns an empty list.
    /// </summary>
    [Fact(DisplayName = "Given valid command When no users exist Then returns an empty list")]
    public async Task Handle_NoUsers_ReturnsEmptyList()
    {
        // Arrange
        int pageNumber = 1;
        int pageSize = 5;
        var command = new GetUsersCommand(pageNumber, pageSize);

        _userRepository.GetAllUsersAsync(pageNumber, pageSize, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new List<User>()));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
        await _userRepository.Received(1).GetAllUsersAsync(pageNumber, pageSize, Arg.Any<CancellationToken>());
    }
}
