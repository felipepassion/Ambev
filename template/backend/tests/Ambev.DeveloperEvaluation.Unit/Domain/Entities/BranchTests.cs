using Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Entities
{
    public class BranchTests
    {
        [Fact(DisplayName = "Should deactivate the branch correctly")]
        public void Given_ActiveBranch_When_Deactivated_Then_IsActiveShouldBeFalse()
        {
            // arrange
            var branch = BranchTestData.GenerateValidBranch();

            // act
            branch.Deactivate();

            // assert
            Assert.False(branch.IsActive);
            Assert.NotNull(branch.UpdatedAt);
        }

        [Fact(DisplayName = "Should validate successfully for valid data")]
        public void Given_ValidBranch_When_Validated_Then_ShouldBeValid()
        {
            // arrange
            var branch = BranchTestData.GenerateValidBranch();

            // act
            var result = branch.Validate();

            // assert
            Assert.True(result.IsValid);
            Assert.Empty(result.Errors);
        }

        [Fact(DisplayName = "Should fail validation for empty name")]
        public void Given_EmptyName_When_Validated_Then_ShouldBeInvalid()
        {
            // arrange
            var branch = BranchTestData.GenerateValidBranch();
            branch.Name = string.Empty;

            // act
            var result = branch.Validate();

            // assert
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.Detail.Contains("Branch name is required"));
        }

        [Fact(DisplayName = "Should fail validation when name exceeds 100 characters")]
        public void Given_TooLongName_When_Validated_Then_ShouldBeInvalid()
        {
            // arrange
            var branch = BranchTestData.GenerateValidBranch();
            branch.Name = new string('A', 101);

            // act
            var result = branch.Validate();

            // assert
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.Detail.Contains("cannot exceed 100 characters"));
        }
    }
}
