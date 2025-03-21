namespace Ambev.DeveloperEvaluation.Domain.Entities;

using Ambev.DeveloperEvaluation.Domain.Validation;
using Ambev.DeveloperEvaluation.Common.Validation;
using Ambev.DeveloperEvaluation.Domain.Common;

/// <summary>
/// Represents an item in a sale.
/// Stores product information, quantity, discount, and other item-related data.
/// </summary>
public class SaleItem : BaseEntity
{
    /// <summary>
    /// Gets or sets the identifier of the parent sale.
    /// </summary>
    public required Guid SaleId { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the product involved in this sale item.
    /// </summary>
    public required Guid ProductId { get; set; }

    /// <summary>
    /// Gets or sets the quantity of this product in the sale.
    /// </summary>
    public required int Quantity { get; set; }

    /// <summary>
    /// Gets or sets the unit price of the product at the time of the sale.
    /// </summary>
    public required decimal UnitPrice { get; set; }

    /// <summary>
    /// Gets or sets the discount percentage applied to this item.
    /// </summary>
    public decimal Discount { get; set; } = 0;

    /// <summary>
    /// Gets or sets the total amount for this item, after discount.
    /// </summary>
    public decimal TotalItemAmount { get; set; }

    /// <summary>
    /// Indicates whether the item has been cancelled.
    /// </summary>
    public bool IsItemCancelled { get; set; }

    /// <summary>
    /// Validates the current SaleItem entity based on predefined rules.
    /// </summary>
    /// <returns>A <see cref="ValidationResultDetail"/> with validation results.</returns>
    public ValidationResultDetail Validate()
    {
        var validator = new SaleItemValidator();
        var result = validator.Validate(this);
        return new ValidationResultDetail
        {
            IsValid = result.IsValid,
            Errors = result.Errors.Select(e => (ValidationErrorDetail)e)
        };
    }

    /// <summary>
    /// Marks this item as cancelled.
    /// </summary>
    public void CancelItem()
    {
        IsItemCancelled = true;
    }
}
