using MediatR;

namespace Ambev.DeveloperEvaluation.Domain.Events.Branches;

/// <summary>
/// Event fired when a branch is successfully created.
/// </summary>
public record BranchCreatedEvent(Guid BranchId, string Name) : INotification;