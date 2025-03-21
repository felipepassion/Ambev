namespace Ambev.DeveloperEvaluation.Domain.Entities;

using Ambev.DeveloperEvaluation.Domain.Validation;
using Ambev.DeveloperEvaluation.Common.Validation;
using Ambev.DeveloperEvaluation.Domain.Common;

/// <summary>
/// Represents a branch (or location) where sales occur.
/// </summary>
public class Branch : BaseEntity
{
    /// <summary>
    /// Gets or sets the branch name.
    /// </summary>
    public required string Name { get; set; } = string.Empty;

    /// <summary>
    /// Indicates whether the branch is active.
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Gets or sets the creation date and time for this branch record.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the branch record was last updated.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Branch"/> class.
    /// </summary>
    public Branch()
    {
        CreatedAt = DateTime.UtcNow;
        IsActive = true;
    }

    /// <summary>
    /// Validates the current Branch entity based on predefined rules.
    /// </summary>
    /// <returns>A <see cref="ValidationResultDetail"/> with validation results.</returns>
    public ValidationResultDetail Validate()
    {
        var validator = new BranchValidator();
        var result = validator.Validate(this);
        return new ValidationResultDetail
        {
            IsValid = result.IsValid,
            Errors = result.Errors.Select(e => (ValidationErrorDetail)e)
        };
    }

    /// <summary>
    /// Marks this branch as inactive.
    /// </summary>
    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }
}
