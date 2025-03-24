using MediatR;

namespace Ambev.DeveloperEvaluation.Domain.Events.Sales;

/// <summary>
/// Represents an event triggered when a sale is created, containing relevant identifiers and items involved in the
/// sale.
/// </summary>
/// <param name="SaleId">Identifies the specific sale that has been created.</param>
/// <param name="BranchId">Indicates the branch where the sale took place.</param>
/// <param name="UserId">Represents the user who initiated the sale.</param>
/// <param name="Items">Contains a list of items associated with the created sale.</param>
public record SaleCreatedEvent(
    Guid SaleId,
    Guid BranchId,
    Guid UserId,
    List<SaleItemCreatedEvent> Items
) : INotification;
