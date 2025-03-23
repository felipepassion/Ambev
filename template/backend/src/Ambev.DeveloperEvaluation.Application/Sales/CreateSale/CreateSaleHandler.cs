using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Ambev.DeveloperEvaluation.Application.Sales.CreateSale;

/// <summary>
/// Handler for processing <see cref="CreateSaleCommand"/> requests.
/// Pulls product info from the database to determine unit price,
/// applies discount logic, and calculates the total amount for the sale.
/// </summary>
public class CreateSaleHandler : IRequestHandler<CreateSaleCommand, CreateSaleResult>
{
    private readonly IHttpContextAccessor _httpFactory;
    private readonly ISaleRepository _saleRepository;
    private readonly IBranchRepository _branchRepository;
    private readonly IProductRepository _productRepository;

    public CreateSaleHandler(
        IHttpContextAccessor factory,
        ISaleRepository saleRepository,
        IBranchRepository branchRepository,
        IProductRepository productRepository)
    {
        _httpFactory = factory;
        _saleRepository = saleRepository;
        _branchRepository = branchRepository;
        _productRepository = productRepository;
    }

    /// <summary>
    /// Handles the <see cref="CreateSaleCommand"/> request by validating input,
    /// retrieving needed data from the database, applying discount logic,
    /// and storing the newly created sale.
    /// </summary>
    /// <param name="command">The create sale command.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created sale details (Id).</returns>
    /// <exception cref="ValidationException">Thrown when validation fails.</exception>
    /// <exception cref="InvalidOperationException">Thrown if the branch or product does not exist.</exception>
    public async Task<CreateSaleResult> Handle(CreateSaleCommand command, CancellationToken cancellationToken)
    {
        var userId = _httpFactory.HttpContext!.User.FindFirst(ClaimTypes.NameIdentifier)!.Value!;

        // 1. Validate command
        var validator = new CreateSaleCommandValidator();
        var validationResult = await validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        // 2. Check if the branch exists
        var branch = await _branchRepository.GetByIdAsync(command.BranchId, cancellationToken);
        if (branch == null)
            throw new InvalidOperationException($"Branch with ID '{command.BranchId}' does not exist.");

        // 3. Create the Sale entity
        var sale = new Sale
        {
            // Generate a sale number if needed; here we just use a GUID
            SaleNumber = Guid.NewGuid().ToString(),
            SaleDate = DateTime.UtcNow,
            BranchId = branch.Id,
            IsCancelled = false,
            CreatedAt = DateTime.UtcNow,
            UserId = Guid.Parse(userId),
            Items = []
        };

        // 4. For each item, fetch product info, apply discount logic, and build SaleItem
        var saleItems = new List<SaleItem>();

        foreach (var itemCmd in command.Items)
        {
            var product = await _productRepository.GetByIdAsync(itemCmd.ProductId, cancellationToken);
            if (product == null)
                throw new InvalidOperationException($"Product with ID '{itemCmd.ProductId}' does not exist.");

            // Determine discount based on quantity
            decimal discountPercent = 0m;
            if (itemCmd.Quantity >= 4 && itemCmd.Quantity < 10)
                discountPercent = 0.10m; // 10%
            else if (itemCmd.Quantity >= 10 && itemCmd.Quantity <= 20)
                discountPercent = 0.20m; // 20%

            // Calculate final price for this item
            var rawTotal = product.UnitPrice * itemCmd.Quantity;
            var discountValue = rawTotal * discountPercent;
            var total = rawTotal - discountValue;

            sale.Items.Add(new SaleItem
            {
                SaleId = sale.Id,
                ProductId = product.Id,
                Quantity = itemCmd.Quantity,
                UnitPrice = product.UnitPrice,
                Discount = discountPercent,
                TotalItemAmount = total,
                IsItemCancelled = false
            });
        }

        // 6. Persist the sale
        var createdSale = await _saleRepository.CreateAsync(sale, cancellationToken);

        // 7. Return result
        return new CreateSaleResult { Id = createdSale.Id };
    }
}
