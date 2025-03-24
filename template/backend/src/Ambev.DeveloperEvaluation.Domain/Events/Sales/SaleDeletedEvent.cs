namespace Ambev.DeveloperEvaluation.Domain.Events.Sales;

/// <summary>
/// Event fired when a sale is successfully deleted.
/// </summary>
public record SaleDeletedEvent(Guid SaleId) : MediatR.INotification;
