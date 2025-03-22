namespace Ambev.DeveloperEvaluation.WebApi.Features.Branches.GetBranch;

/// <summary>
/// Represents the response after successfully retrieving a branch.
/// </summary>
public class GetBranchResponse
{
    /// <summary>
    /// Gets or sets the unique identifier of the branch.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the name of the branch.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Indicates whether the branch is currently active.
    /// </summary>
    public bool IsActive { get; set; }
}
