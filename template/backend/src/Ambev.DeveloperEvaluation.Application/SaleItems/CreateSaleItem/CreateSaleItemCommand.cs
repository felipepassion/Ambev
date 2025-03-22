using Ambev.DeveloperEvaluation.Common.Validation;
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
    /// Gets or sets the product identifier for this sale item.
    /// </summary>
    public Guid ProductId { get; set; }

    /// <summary>
    /// Gets or sets the quantity of this product to be purchased.
    /// </summary>
    public int Quantity { get; set; }

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