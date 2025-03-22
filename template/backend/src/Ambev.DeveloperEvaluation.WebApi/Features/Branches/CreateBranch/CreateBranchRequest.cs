namespace Ambev.DeveloperEvaluation.WebApi.Features.Branches.CreateBranch;

/// <summary>
/// Request object for creating a branch.
/// </summary>
public class CreateBranchRequest
{
    /// <summary>
    /// Gets or sets the name of the branch to be created.
    /// </summary>
    public string Name { get; set; } = string.Empty;
}
