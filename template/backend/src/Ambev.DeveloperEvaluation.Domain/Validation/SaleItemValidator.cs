namespace Ambev.DeveloperEvaluation.Domain.Validation;

using Ambev.DeveloperEvaluation.Domain.Entities;
using FluentValidation;

/// <summary>
/// Provides validation rules for the <see cref="SaleItem"/> entity.
/// Enforces quantity limits (1-20), discount tiers (0%, 10%, 20%), and unit price constraints.
/// </summary>
public class SaleItemValidator : AbstractValidator<SaleItem>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SaleItemValidator"/> class with specific validation rules.
    /// </summary>
    public SaleItemValidator()
    {
        RuleFor(i => i.ProductId)
            .NotEmpty()
            .WithMessage("Product ID is required.");

        RuleFor(i => i.Quantity)
            .GreaterThan(0).WithMessage("Quantity must be at least 1.")
            .LessThanOrEqualTo(20).WithMessage("Quantity cannot exceed 20.");

        RuleFor(i => i.UnitPrice)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Unit price cannot be negative.");

        // Discount rules based on quantity
        When(i => i.Quantity < 4, () =>
        {
            RuleFor(i => i.Discount)
                .Equal(0)
                .WithMessage("No discount allowed for quantities below 4.");
        });

        When(i => i.Quantity >= 4 && i.Quantity < 10, () =>
        {
            RuleFor(i => i.Discount)
                .Equal(0.1m)
                .WithMessage("Discount must be 10% for quantities between 4 and 9.");
        });

        When(i => i.Quantity >= 10 && i.Quantity <= 20, () =>
        {
            RuleFor(i => i.Discount)
                .Equal(0.2m)
                .WithMessage("Discount must be 20% for quantities between 10 and 20.");
        });
    }
}
