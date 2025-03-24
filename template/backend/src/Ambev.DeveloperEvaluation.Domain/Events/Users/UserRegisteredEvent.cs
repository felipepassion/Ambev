using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Domain.Events.Users;

/// <summary>
/// Represents an event that occurs when a user registers. Contains the registered user's information.
/// </summary>
public record UserRegisteredEvent : MediatR.INotification
{
    public User User { get; }

    public UserRegisteredEvent(User user)
    {
        User = user;
    }
}
