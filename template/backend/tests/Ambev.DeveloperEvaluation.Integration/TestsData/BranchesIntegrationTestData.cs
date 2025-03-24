using Bogus;
using Ambev.DeveloperEvaluation.WebApi.Features.Branches.CreateBranch;

namespace Ambev.DeveloperEvaluation.Integration.TestsData
{
    public static class BranchesIntegrationTestData
    {
        private static readonly Faker<CreateBranchRequest> _branchFaker =
            new Faker<CreateBranchRequest>()
                .RuleFor(r => r.Name, f => f.Company.CompanyName());

        public static CreateBranchRequest GenerateValidCreateBranchRequest()
        {
            return _branchFaker.Generate();
        }

        public static CreateBranchRequest GenerateInvalidCreateBranchRequest()
        {
            return new CreateBranchRequest
            {
                Name = string.Empty
            };
        }
    }
}
