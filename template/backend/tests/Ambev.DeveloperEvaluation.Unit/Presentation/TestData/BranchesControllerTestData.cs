using Bogus;
using Ambev.DeveloperEvaluation.WebApi.Features.Branches.CreateBranch;

namespace Ambev.DeveloperEvaluation.Unit.Presentation.TestData;

/// <summary>
/// Provides methods for generating test data (requests) for the BranchesController.
/// </summary>
public static class BranchesControllerTestData
{
    private static readonly Faker<CreateBranchRequest> _createBranchFaker =
        new Faker<CreateBranchRequest>()
            .RuleFor(r => r.Name, f => f.Company.CompanyName());

    /// <summary>
    /// Generates a valid CreateBranchRequest with randomized data.
    /// </summary>
    public static CreateBranchRequest GenerateValidCreateBranchRequest()
    {
        return _createBranchFaker.Generate();
    }

    /// <summary>
    /// Generates an invalid CreateBranchRequest that fails validation.
    /// </summary>
    public static CreateBranchRequest GenerateInvalidCreateBranchRequest()
    {
        return new CreateBranchRequest
        {
            Name = "" // Required => invalid
        };
    }
}
