using Ambev.DeveloperEvaluation.Application.Users.DeleteUser;
using Ambev.DeveloperEvaluation.Domain.Events.Users;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using FluentAssertions;
using MediatR;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Tests.Unit.Application.Users;

/// <summary>
/// Contains unit tests for the <see cref="DeleteUserHandler"/> class.
/// </summary>
public class DeleteUserHandlerTests
{
    private readonly IUserRepository _userRepository;
    private readonly IMediator _mediator;
    private readonly DeleteUserHandler _handler;

    public DeleteUserHandlerTests()
    {
        _userRepository = Substitute.For<IUserRepository>();
        _mediator = Substitute.For<IMediator>();
        _handler = new DeleteUserHandler(_userRepository, _mediator);
    }

    /// <summary>
    /// Tests that a valid user deletion request returns a success response and publishes a UserDeletedEvent.
    /// </summary>
    [Fact(DisplayName = "Given valid user ID When deleting user Then returns success and publishes UserDeletedEvent")]
    public async Task Handle_ValidCommand_PublishesUserDeletedEvent()
    {
        // Given
        var userId = Guid.NewGuid();
        var command = new DeleteUserCommand(userId);

        _userRepository.DeleteAsync(userId, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(true));

        // When
        var result = await _handler.Handle(command, CancellationToken.None);

        // Then
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();

        // Verify that the mediator publishes a UserDeletedEvent with the correct user ID
        await _mediator.Received(1)
            .Publish(Arg.Is<UserDeletedEvent>(e => e.UserId == userId), Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Tests that a deletion request for a non-existent user throws a KeyNotFoundException.
    /// </summary>
    [Fact(DisplayName = "Given non-existent user When deleting user Then throws KeyNotFoundException")]
    public async Task Handle_NonExistentUser_ThrowsKeyNotFoundException()
    {
        // Given
        var userId = Guid.NewGuid();
        var command = new DeleteUserCommand(userId);

        _userRepository.DeleteAsync(userId, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(false));

        // When & Then
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _handler.Handle(command, CancellationToken.None));
    }
}
