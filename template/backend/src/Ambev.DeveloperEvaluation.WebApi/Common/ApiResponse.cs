using Ambev.DeveloperEvaluation.Common.Validation;

namespace Ambev.DeveloperEvaluation.WebApi.Common;

public class ApiResponse
{
    public bool Success { get; set; }
    public string? Message { get; set; } = string.Empty;
    public List<ValidationErrorDetail> Errors { get; set; } = new List<ValidationErrorDetail>();
}
