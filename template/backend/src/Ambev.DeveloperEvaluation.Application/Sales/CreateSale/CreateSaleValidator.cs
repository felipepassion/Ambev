using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Sales.CreateSale;

/// <summary>
/// Validator for <see cref="CreateSaleCommand"/> that defines basic validation rules for creating a sale.
/// </summary>
public class CreateSaleCommandValidator : AbstractValidator<CreateSaleCommand>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CreateSaleCommandValidator"/> class.
    /// </summary>
    public CreateSaleCommandValidator()
    {
        RuleFor(cmd => cmd.BranchId)
            .NotEmpty()
            .WithMessage("BranchId is required.");

        RuleFor(cmd => cmd.Items)
            .NotNull().WithMessage("Items list must not be null.")
            .NotEmpty().WithMessage("At least one item is required to create a sale.");

        RuleForEach(cmd => cmd.Items)
            .ChildRules(item =>
            {
                item.RuleFor(i => i.ProductId)
                    .NotEmpty()
                    .WithMessage("ProductId is required.");

                item.RuleFor(i => i.Quantity)
                    .GreaterThan(0).WithMessage("Quantity must be at least 1.")
                    .LessThanOrEqualTo(20).WithMessage("Cannot sell more than 20 items of the same product.");
            });

        // Per-item rules: each line must not exceed 20
        RuleForEach(cmd => cmd.Items)
            .ChildRules(item =>
            {
                item.RuleFor(i => i.ProductId)
                    .NotEmpty()
                    .WithMessage("ProductId is required.");

                item.RuleFor(i => i.Quantity)
                    .GreaterThan(0)
                    .WithMessage("Quantity must be at least 1.")
                    .LessThanOrEqualTo(20)
                    .WithMessage("Cannot sell more than 20 items of the same product in a single line.");
            });

        // Aggregate rule: ensure that if the same product ID is listed multiple times,
        // the sum of quantities does not exceed 20.
        RuleFor(cmd => cmd).Custom((command, context) =>
        {
            var groupedByProduct = command.Items
                                          .GroupBy(x => x.ProductId)
                                          .ToList();

            foreach (var group in groupedByProduct)
            {
                var totalQuantityForProduct = group.Sum(i => i.Quantity);
                if (totalQuantityForProduct > 20)
                {
                    context.AddFailure(
                        "Items",
                        $"You cannot sell more than 20 items total of the same product (ProductId: {group.Key})."
                    );
                }
            }
        });
    }
}
