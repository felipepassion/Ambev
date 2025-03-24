using MediatR;

namespace Ambev.DeveloperEvaluation.Domain.Events.Products;

/// <summary>
/// Event fired when a product is successfully created.
/// </summary>
public record ProductCreatedEvent(
    Guid ProductId,
    string Name,
    string? Description,
    decimal UnitPrice
) : INotification;
