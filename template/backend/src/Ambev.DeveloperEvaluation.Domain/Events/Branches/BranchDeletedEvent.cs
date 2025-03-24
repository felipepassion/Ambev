using MediatR;

namespace Ambev.DeveloperEvaluation.Domain.Events.Branches;

/// <summary>
/// Event fired when a branch is successfully deleted.
/// </summary>
public record BranchDeletedEvent(Guid BranchId) : INotification;