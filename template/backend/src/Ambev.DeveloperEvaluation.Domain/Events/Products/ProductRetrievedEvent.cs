using MediatR;

namespace Ambev.DeveloperEvaluation.Domain.Events.Products;

/// <summary>
/// Event fired when a product is successfully retrieved.
/// </summary>
public record ProductRetrievedEvent(Guid ProductId) : INotification;
