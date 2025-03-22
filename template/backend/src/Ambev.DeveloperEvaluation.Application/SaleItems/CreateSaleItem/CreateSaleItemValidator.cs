using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.SaleItems.CreateSaleItem;

/// <summary>
/// Validator for <see cref="CreateSaleItemCommand"/>, defining validation rules for saleitem creation.
/// </summary>
/// <remarks>
/// Validation rules include:
/// - Name: Required, must not exceed 100 characters
/// </remarks>
public class CreateSaleItemCommandValidator : AbstractValidator<CreateSaleItemCommand>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CreateSaleItemCommandValidator"/> class.
    /// </summary>
    public CreateSaleItemCommandValidator()
    {
        RuleFor(cmd => cmd.Name)
            .NotEmpty().WithMessage("SaleItem name cannot be empty.")
            .MaximumLength(100).WithMessage("SaleItem name cannot exceed 100 characters.");
    }
}
