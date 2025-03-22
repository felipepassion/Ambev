using Ambev.DeveloperEvaluation.Common.Validation;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Branchs.CreateBranch;

/// <summary>
/// Command for creating a new branch.
/// </summary>
/// <remarks>
/// This command is used to capture the required data for creating a branch, 
/// including branchname, password, phone number, email, status, and role. 
/// It implements <see cref="IRequest{TResponse}"/> to initiate the request 
/// that returns a <see cref="CreateBranchResult"/>.
/// 
/// The data provided in this command is validated using the 
/// <see cref="CreateBranchCommandValidator"/> which extends 
/// <see cref="AbstractValidator{T}"/> to ensure that the fields are correctly 
/// populated and follow the required rules.
/// </remarks>
public class CreateBranchCommand : IRequest<CreateBranchResult>
{
    /// <summary>
    /// Gets or sets the name for the branch.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    public ValidationResultDetail Validate()
    {
        var validator = new CreateBranchCommandValidator();
        var result = validator.Validate(this);
        return new ValidationResultDetail
        {
            IsValid = result.IsValid,
            Errors = result.Errors.Select(o => (ValidationErrorDetail)o)
        };
    }
}