using Bogus;
using Ambev.DeveloperEvaluation.WebApi.Features.Branches.CreateBranch;

namespace Ambev.DeveloperEvaluation.Unit.Presentation.TestData
{
    /// <summary>
    /// Provides methods for generating test data for Branch integration tests.
    /// </summary>
    public static class BranchesIntegrationTestData
    {
        private static readonly Faker<CreateBranchRequest> _branchFaker =
            new Faker<CreateBranchRequest>()
                .RuleFor(r => r.Name, f => f.Company.CompanyName());

        /// <summary>
        /// Generates a valid CreateBranchRequest with randomized data.
        /// </summary>
        /// <returns>A valid CreateBranchRequest object.</returns>
        public static CreateBranchRequest GenerateValidCreateBranchRequest()
        {
            return _branchFaker.Generate();
        }

        /// <summary>
        /// Generates an invalid CreateBranchRequest with missing required data.
        /// </summary>
        /// <returns>An invalid CreateBranchRequest object.</returns>
        public static CreateBranchRequest GenerateInvalidCreateBranchRequest()
        {
            return new CreateBranchRequest
            {
                Name = string.Empty // Name is required, so this is invalid.
            };
        }
    }
}
