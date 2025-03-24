using MediatR;

namespace Ambev.DeveloperEvaluation.Domain.Events;

/// <summary>
/// Event fired when a branch is retrieved.
/// </summary>
public record BranchRetrievedEvent(Guid BranchId) : INotification;