using MediatR;

namespace Ambev.DeveloperEvaluation.Domain.Events;

public record SaleItemCreatedEvent(
    Guid ProductId,
    int Quantity,
    decimal Discount,
    decimal TotalItemAmount
);

public record SaleCreatedEvent(
    Guid SaleId,
    Guid BranchId,
    Guid UserId,
    List<SaleItemCreatedEvent> Items
) : INotification;
