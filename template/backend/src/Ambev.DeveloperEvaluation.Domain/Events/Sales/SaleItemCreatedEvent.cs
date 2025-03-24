namespace Ambev.DeveloperEvaluation.Domain.Events.Sales;

/// <summary>
/// Represents an event triggered when a sale item is created, containing details about the product and transaction.
/// </summary>
/// <param name="ProductId">Identifies the specific product involved in the sale.</param>
/// <param name="Quantity">Indicates the number of units sold in the transaction.</param>
/// <param name="Discount">Specifies any discount applied to the sale item.</param>
/// <param name="TotalItemAmount">Represents the total monetary value of the sale item after applying discounts.</param>
public record SaleItemCreatedEvent(
    Guid ProductId,
    int Quantity,
    decimal Discount,
    decimal TotalItemAmount
) : MediatR.INotification;
