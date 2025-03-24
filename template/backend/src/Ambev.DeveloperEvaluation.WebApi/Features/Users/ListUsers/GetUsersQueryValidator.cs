using FluentValidation;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Users.ListUsers;

/// <summary>
/// Validator for GetUsersQuery.
/// </summary>
public class GetUsersQueryValidator : AbstractValidator<GetUsersQuery>
{
    /// <summary>
    /// Initializes validation rules for GetUsersQuery.
    /// </summary>
    public GetUsersQueryValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThan(0)
            .WithMessage("Page number must be greater than zero");

        RuleFor(x => x.PageSize)
            .GreaterThan(0)
            .WithMessage("Page size must be greater than zero");
    }
}
