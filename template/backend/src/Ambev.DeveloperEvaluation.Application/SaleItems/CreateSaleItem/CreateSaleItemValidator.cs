using Ambev.DeveloperEvaluation.Application.SaleItems.CreateSaleItem;
using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Sales.CreateSale;

/// <summary>
/// Validator for <see cref="CreateSaleItemCommand"/>, defining
/// basic rules for each item in a sale.
/// </summary>
public class CreateSaleItemCommandValidator : AbstractValidator<CreateSaleItemCommand>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CreateSaleItemCommandValidator"/> class.
    /// </summary>
    public CreateSaleItemCommandValidator()
    {
        RuleFor(i => i.ProductId)
            .NotEqual(Guid.Empty)
            .WithMessage("ProductId is required.");

        RuleFor(i => i.Quantity)
            .GreaterThan(0).WithMessage("Quantity must be at least 1.")
            .LessThanOrEqualTo(20).WithMessage("Cannot sell more than 20 items of the same product.");
    }
}
