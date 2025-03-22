using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Products.CreateProduct
{
    /// <summary>
    /// Validator for <see cref="CreateProductCommand"/>, defining validation rules for product creation.
    /// </summary>
    public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
    {
        /// <summary>
        /// Initializes a new instance of <see cref="CreateProductCommandValidator"/> with custom rules.
        /// </summary>
        public CreateProductCommandValidator()
        {
            RuleFor(cmd => cmd.Name)
                .NotEmpty().WithMessage("Product name cannot be empty.")
                .MaximumLength(100).WithMessage("Product name cannot exceed 100 characters.");

            RuleFor(cmd => cmd.Description)
                .MaximumLength(500).WithMessage("Description cannot exceed 500 characters.")
                .When(cmd => !string.IsNullOrEmpty(cmd.Description));

            RuleFor(cmd => cmd.UnitPrice)
                .GreaterThanOrEqualTo(0).WithMessage("Unit price cannot be negative.");
        }
    }
}
