namespace Ambev.DeveloperEvaluation.Domain.Events.Sales;

/// <summary>
/// Event fired when a sale is retrieved.
/// </summary>
public record SaleRetrievedEvent(Guid SaleId) : MediatR.INotification;
