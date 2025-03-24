namespace Ambev.DeveloperEvaluation.Domain.Events.Sales;

/// <summary>
/// Event fired when a sale is successfully canceled.
/// </summary>
public record SaleCanceledEvent(Guid SaleId) : MediatR.INotification;
