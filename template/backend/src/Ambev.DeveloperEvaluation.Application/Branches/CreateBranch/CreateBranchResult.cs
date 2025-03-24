namespace Ambev.DeveloperEvaluation.Application.Branches.CreateBranch;

/// <summary>
/// Represents the response returned after successfully creating a new branch.
/// </summary>
public class CreateBranchResult
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
