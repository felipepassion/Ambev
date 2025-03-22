namespace Ambev.DeveloperEvaluation.WebApi.Features.Branches.CreateBranch;

/// <summary>
/// Represents the response after successfully creating a new branch.
/// </summary>
public class CreateBranchResponse
{
    /// <summary>
    /// Gets or sets the unique identifier of the newly created branch.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the name of the newly created branch.
    /// </summary>
    public string Name { get; set; } = string.Empty;
}
