namespace Ambev.DeveloperEvaluation.Domain.Validation;

using Ambev.DeveloperEvaluation.Domain.Entities;
using FluentValidation;

/// <summary>
/// Provides validation rules for the <see cref="Sale"/> entity.
/// Ensures that the sale has a valid number, date, total amount, and at least one item.
/// </summary>
public class SaleValidator : AbstractValidator<Sale>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SaleValidator"/> class with specific validation rules.
    /// </summary>
    public SaleValidator()
    {
        RuleFor(s => s.SaleNumber)
            .NotEmpty()
            .WithMessage("Sale number cannot be empty.");

        RuleFor(s => s.SaleDate)
            .NotEmpty()
            .WithMessage("Sale date is required.");

        RuleFor(s => s.TotalAmount)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Total amount cannot be negative.");

        RuleFor(s => s.Items)
            .NotNull()
            .NotEmpty()
            .WithMessage("Sale must contain at least one item.");
    }
}
