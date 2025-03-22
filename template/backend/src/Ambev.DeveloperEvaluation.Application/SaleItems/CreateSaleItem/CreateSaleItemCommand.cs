using Ambev.DeveloperEvaluation.Common.Validation;
using Ambev.DeveloperEvaluation.Domain.Enums;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.SaleItems.CreateSaleItem;

/// <summary>
/// Command for creating a new saleitem.
/// </summary>
/// <remarks>
/// This command is used to capture the required data for creating a saleitem, 
/// including saleitemname, password, phone number, email, status, and role. 
/// It implements <see cref="IRequest{TResponse}"/> to initiate the request 
/// that returns a <see cref="CreateSaleItemResult"/>.
/// 
/// The data provided in this command is validated using the 
/// <see cref="CreateSaleItemCommandValidator"/> which extends 
/// <see cref="AbstractValidator{T}"/> to ensure that the fields are correctly 
/// populated and follow the required rules.
/// </remarks>
public class CreateSaleItemCommand : IRequest<CreateSaleItemResult>
{
    /// <summary>
    /// Gets or sets the name for the saleitem.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    public ValidationResultDetail Validate()
    {
        var validator = new CreateSaleItemCommandValidator();
        var result = validator.Validate(this);
        return new ValidationResultDetail
        {
            IsValid = result.IsValid,
            Errors = result.Errors.Select(o => (ValidationErrorDetail)o)
        };
    }
}