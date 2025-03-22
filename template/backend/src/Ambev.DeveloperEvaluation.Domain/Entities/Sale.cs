namespace Ambev.DeveloperEvaluation.Domain.Entities;

using Ambev.DeveloperEvaluation.Domain.Validation;
using Ambev.DeveloperEvaluation.Common.Validation;
using Ambev.DeveloperEvaluation.Domain.Common;
using System.ComponentModel.DataAnnotations.Schema;

/// <summary>
/// Aggregate root entity for sales.
/// Stores information about the sale itself, the user, branch, and sale items.
/// </summary>
public class Sale : BaseEntity
{
    /// <summary>
    /// Gets or sets the unique sale number.
    /// </summary>
    public required string SaleNumber { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the sale occurred.
    /// </summary>
    public required DateTime SaleDate { get; set; }

    /// <summary>
    /// Gets or sets the user identifier associated with the sale.
    /// </summary>
    public required Guid UserId { get; set; }

    /// <summary>
    /// Gets or sets the branch identifier where the sale occurred.
    /// </summary>
    [ForeignKey(nameof(Branch))]
    public required Guid BranchId { get; set; }

    /// <summary>
    /// Indicates whether the sale has been cancelled.
    /// </summary>
    public bool IsCancelled { get; set; }

    /// <summary>
    /// Gets or sets the total amount of the entire sale.
    /// </summary>
    [Column("TotalAmount")]
    private decimal _totalAmount;
    public decimal TotalAmount => (_totalAmount = this.Items.Sum(x => x.TotalItemAmount));

    /// <summary>
    /// Gets or sets the creation date and time for the sale record.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the sale record was last updated.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Collection of items related to this sale.
    /// </summary>
    public required virtual List<SaleItem> Items { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public virtual Branch Branch { get; set; } = default!;

    /// <summary>
    /// Initializes a new instance of the <see cref="Sale"/> class.
    /// </summary>
    public Sale()
    {
        CreatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Performs validation for the current Sale entity based on predefined rules.
    /// </summary>
    /// <returns>A <see cref="ValidationResultDetail"/> with validation results.</returns>
    public ValidationResultDetail Validate()
    {
        var validator = new SaleValidator();
        var result = validator.Validate(this);
        return new ValidationResultDetail
        {
            IsValid = result.IsValid,
            Errors = result.Errors.Select(e => (ValidationErrorDetail)e)
        };
    }

    /// <summary>
    /// Marks the sale as cancelled.
    /// </summary>
    public void Cancel()
    {
        IsCancelled = true;
        UpdatedAt = DateTime.UtcNow;
    }
}
