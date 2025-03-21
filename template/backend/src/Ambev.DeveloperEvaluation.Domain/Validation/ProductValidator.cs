namespace Ambev.DeveloperEvaluation.Domain.Validation;

using Ambev.DeveloperEvaluation.Domain.Entities;
using FluentValidation;

/// <summary>
/// Provides validation rules for the <see cref="Product"/> entity.
/// Checks that the product has a valid name and a non-negative unit price.
/// </summary>
public class ProductValidator : AbstractValidator<Product>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ProductValidator"/> class with specific validation rules.
    /// </summary>
    public ProductValidator()
    {
        RuleFor(p => p.Name)
            .NotEmpty()
            .WithMessage("Product name is required.")
            .MaximumLength(100)
            .WithMessage("Product name cannot exceed 100 characters.");

        RuleFor(p => p.UnitPrice)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Unit price cannot be negative.");
    }
}
