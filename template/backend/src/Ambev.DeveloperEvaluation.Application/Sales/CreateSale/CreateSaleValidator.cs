using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Sales.CreateSale;

/// <summary>
/// Validator for <see cref="CreateSaleCommand"/>, defining validation rules for sale creation.
/// </summary>
/// <remarks>
/// Validation rules include:
/// - Name: Required, must not exceed 100 characters
/// </remarks>
public class CreateSaleCommandValidator : AbstractValidator<CreateSaleCommand>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CreateSaleCommandValidator"/> class.
    /// </summary>
    public CreateSaleCommandValidator()
    {
        RuleFor(cmd => cmd.Name)
            .NotEmpty().WithMessage("Sale name cannot be empty.")
            .MaximumLength(100).WithMessage("Sale name cannot exceed 100 characters.");
    }
}
