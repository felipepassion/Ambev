using MediatR;

namespace Ambev.DeveloperEvaluation.Domain.Events.Products;

/// <summary>
/// Event fired when a product is successfully deleted.
/// </summary>
public record ProductDeletedEvent(Guid ProductId) : INotification;
