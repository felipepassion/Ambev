using Ambev.DeveloperEvaluation.Application.Users.GetUser;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Events.Users;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using FluentAssertions;
using MediatR;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Tests.Unit.Application.Users;

/// <summary>
/// Contains unit tests for the <see cref="GetUserHandler"/> class.
/// </summary>
public class GetUserHandlerTests
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly IMediator _mediator;
    private readonly GetUserHandler _handler;

    public GetUserHandlerTests()
    {
        _userRepository = Substitute.For<IUserRepository>();
        _mapper = Substitute.For<IMapper>();
        _mediator = Substitute.For<IMediator>();
        _handler = new GetUserHandler(_userRepository, _mapper, _mediator);
    }

    /// <summary>
    /// Tests that a valid user retrieval request returns the correct user details and publishes a UserRetrievedEvent.
    /// </summary>
    [Fact(DisplayName = "Given valid user ID When retrieving user Then returns user details and publishes UserRetrievedEvent")]
    public async Task Handle_ValidCommand_PublishesUserRetrievedEvent()
    {
        // Given
        var userId = Guid.NewGuid();
        var user = new User
        {
            Id = userId,
            Username = "TestUser",
            Email = "test@example.com",
            Phone = "123456789"
            // Add additional properties if needed
        };
        var command = new GetUserCommand(userId);

        _userRepository.GetByIdAsync(userId, Arg.Any<CancellationToken>())!
            .Returns(Task.FromResult(user));

        var mappedResult = new GetUserResult
        {
            Id = userId,
            Username = user.Username,
            Email = user.Email,
            Phone = user.Phone
        };
        _mapper.Map<GetUserResult>(user).Returns(mappedResult);

        // When
        var result = await _handler.Handle(command, CancellationToken.None);

        // Then
        result.Should().NotBeNull();
        result.Id.Should().Be(userId);
        result.Username.Should().Be(user.Username);

        // Verify that the mediator publishes a UserRetrievedEvent with the correct user ID
        await _mediator.Received(1)
            .Publish(Arg.Is<UserRetrievedEvent>(e => e.UserId == userId), Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Tests that a retrieval request for a non-existent user throws a KeyNotFoundException.
    /// </summary>
    [Fact(DisplayName = "Given non-existent user When retrieving user Then throws KeyNotFoundException")]
    public async Task Handle_NonExistentUser_ThrowsKeyNotFoundException()
    {
        // Given
        var userId = Guid.NewGuid();
        var command = new GetUserCommand(userId);

        _userRepository.GetByIdAsync(userId, Arg.Any<CancellationToken>())!
            .Returns(Task.FromResult<User>(null!));

        // When & Then
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _handler.Handle(command, CancellationToken.None));
    }
}
