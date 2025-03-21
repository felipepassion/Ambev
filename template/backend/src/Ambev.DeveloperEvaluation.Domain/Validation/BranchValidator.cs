namespace Ambev.DeveloperEvaluation.Domain.Validation;

using Ambev.DeveloperEvaluation.Domain.Entities;
using FluentValidation;

/// <summary>
/// Provides validation rules for the <see cref="Branch"/> entity.
/// Ensures the branch has a valid name and does not exceed maximum length.
/// </summary>
public class BranchValidator : AbstractValidator<Branch>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BranchValidator"/> class with specific validation rules.
    /// </summary>
    public BranchValidator()
    {
        RuleFor(b => b.Name)
            .NotEmpty()
            .WithMessage("Branch name is required.")
            .MaximumLength(100)
            .WithMessage("Branch name cannot exceed 100 characters.");
    }
}
