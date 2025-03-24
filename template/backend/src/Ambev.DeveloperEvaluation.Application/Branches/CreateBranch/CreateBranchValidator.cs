using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Branches.CreateBranch;

/// <summary>
/// Validator for <see cref="CreateBranchCommand"/>, defining validation rules for branch creation.
/// </summary>
public class CreateBranchCommandValidator : AbstractValidator<CreateBranchCommand>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CreateBranchCommandValidator"/> class.
    /// </summary>
    public CreateBranchCommandValidator()
    {
        RuleFor(cmd => cmd.Name)
            .NotEmpty().WithMessage("Branch name cannot be empty.")
            .MaximumLength(100).WithMessage("Branch name cannot exceed 100 characters.");
    }
}
