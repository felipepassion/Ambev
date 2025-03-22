using Ambev.DeveloperEvaluation.Application.SaleItems.CreateSaleItem;
using Ambev.DeveloperEvaluation.Common.Validation;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.CreateSale;

/// <summary>
/// Command for creating a new sale.
/// </summary>
/// <remarks>
/// This command is used to capture the minimum data required for creating a sale,
/// including which branch is responsible for the sale (BranchId) and
/// a list of items (each contains the product and the quantity).
/// 
/// The actual unit price and any discount logic should be retrieved and applied
/// within the handler (or domain) based on the product ID and business rules.
/// </remarks>
public class CreateSaleCommand : IRequest<CreateSaleResult>
{
    /// <summary>
    /// Gets or sets the branch identifier responsible for this sale.
    /// </summary>
    public Guid BranchId { get; set; }

    /// <summary>
    /// Gets or sets a collection of items to be purchased in this sale.
    /// Each item should specify at least the product identifier and the quantity.
    /// </summary>
    public List<CreateSaleItemCommand> Items { get; set; } = new();

    /// <summary>
    /// Performs validation on this command using <see cref="CreateSaleCommandValidator"/>.
    /// </summary>
    /// <returns>A <see cref="ValidationResultDetail"/> containing validation results.</returns>
    public ValidationResultDetail Validate()
    {
        var validator = new CreateSaleCommandValidator();
        var result = validator.Validate(this);
        return new ValidationResultDetail
        {
            IsValid = result.IsValid,
            Errors = result.Errors.Select(o => (ValidationErrorDetail)o)
        };
    }
}