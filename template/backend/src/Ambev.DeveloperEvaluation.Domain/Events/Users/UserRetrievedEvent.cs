using MediatR;

namespace Ambev.DeveloperEvaluation.Domain.Events.Users;

/// <summary>
/// Event fired when a user is successfully retrieved.
/// </summary>
public record UserRetrievedEvent(Guid UserId) : INotification;
