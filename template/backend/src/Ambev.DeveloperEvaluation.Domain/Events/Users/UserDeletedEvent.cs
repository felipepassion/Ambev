using MediatR;

namespace Ambev.DeveloperEvaluation.Domain.Events.Users;

/// <summary>
/// Event fired when a user is successfully deleted.
/// </summary>
public record UserDeletedEvent(Guid UserId) : INotification;
