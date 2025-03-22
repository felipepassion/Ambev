using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Branchs.DeleteBranch;

/// <summary>
/// Validator for DeleteBranchCommand
/// </summary>
public class DeleteBranchValidator : AbstractValidator<DeleteBranchCommand>
{
    /// <summary>
    /// Initializes validation rules for DeleteBranchCommand
    /// </summary>
    public DeleteBranchValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Branch ID is required");
    }
}
