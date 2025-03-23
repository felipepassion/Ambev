using Ambev.DeveloperEvaluation.Application.Branches.CreateBranch;
using Bogus;

namespace Ambev.DeveloperEvaluation.Unit.Application.TestData
{
    /// <summary>
    /// Provides methods for generating test data for CreateBranchCommand using the Bogus library.
    /// </summary>
    public static class CreateBranchHandlerTestData
    {
        private static readonly Faker<CreateBranchCommand> _branchFaker =
            new Faker<CreateBranchCommand>()
                .RuleFor(b => b.Name, f => f.Company.CompanyName());

        /// <summary>
        /// Generates a valid CreateBranchCommand with randomized data.
        /// </summary>
        public static CreateBranchCommand GenerateValidCommand()
        {
            return _branchFaker.Generate();
        }

        /// <summary>
        /// Generates an invalid CreateBranchCommand (e.g., empty name).
        /// </summary>
        public static CreateBranchCommand GenerateInvalidCommand()
        {
            return new CreateBranchCommand
            {
                Name = "" // Fails validation because name cannot be empty
            };
        }
    }
}
