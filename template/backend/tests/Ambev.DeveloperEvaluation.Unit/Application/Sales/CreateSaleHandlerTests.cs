using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Events.Sales;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Unit.Application.TestData;
using FluentAssertions;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using NSubstitute;
using System.Security.Claims;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales;

/// <summary>
/// Contains unit tests for the <see cref="CreateSaleHandler"/> class.
/// </summary>
public class CreateSaleHandlerTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly IBranchRepository _branchRepository;
    private readonly IProductRepository _productRepository;
    private readonly IMediator _mediator;
    private readonly CreateSaleHandler _handler;

    public CreateSaleHandlerTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _branchRepository = Substitute.For<IBranchRepository>();
        _productRepository = Substitute.For<IProductRepository>();
        _mediator = Substitute.For<IMediator>();

        _handler = new CreateSaleHandler(InitializeHttpContextAccessor(), _saleRepository, _branchRepository, _productRepository, _mediator);
    }

    [Fact(DisplayName = "Given valid sale data When creating sale Then returns success response")]
    public async Task Handle_ValidRequest_ReturnsSuccessResponse()
    {
        // Arrange
        var command = CreateSaleHandlerTestData.GenerateValidCommand();

        var branch = new Branch { Id = command.BranchId, Name = "Test Branch", IsActive = true };
        _branchRepository.GetByIdAsync(command.BranchId, default).Returns(branch);

        foreach (var item in command.Items)
        {
            var product = new Product
            {
                Id = item.ProductId,
                Name = "Test Product",
                UnitPrice = 50m,
                IsActive = true
            };
            _productRepository.GetByIdAsync(item.ProductId, default).Returns(product);
        }

        var sale = new Sale
        {
            Id = Guid.NewGuid(),
            BranchId = command.BranchId,
            Items = [],
            SaleDate = DateTime.UtcNow,
            SaleNumber = Guid.NewGuid().ToString(),
            UserId = Guid.NewGuid()
        };
        _saleRepository.CreateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>()).Returns(sale);

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(sale.Id);

        await _mediator.Received(1).Publish(Arg.Any<SaleCreatedEvent>(), Arg.Any<CancellationToken>());
        await _saleRepository.Received(1).CreateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Given invalid sale data When creating sale Then throws validation exception")]
    public async Task Handle_InvalidRequest_ThrowsValidationException()
    {
        // Arrange
        var invalidCommand = CreateSaleHandlerTestData.GenerateInvalidCommand();

        // Act
        Func<Task> act = () => _handler.Handle(invalidCommand, default);

        // Assert
        await act.Should().ThrowAsync<ValidationException>();
    }

    [Fact(DisplayName = "Given non-existent branch When creating sale Then throws invalid operation exception")]
    public async Task Handle_BranchDoesNotExist_ThrowsInvalidOperationException()
    {
        // Arrange
        var command = CreateSaleHandlerTestData.GenerateValidCommand();

        // Act
        Func<Task> act = () => _handler.Handle(command, default);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage($"Branch with ID '{command.BranchId}' does not exist.");
    }

    [Fact(DisplayName = "Given non-existent product When creating sale Then throws invalid operation exception")]
    public async Task Handle_ProductDoesNotExist_ThrowsInvalidOperationException()
    {
        // Arrange
        var command = CreateSaleHandlerTestData.GenerateValidCommand();
        // the branch exists
        var branch = new Branch
        {
            Name = "Test Branch",
            Id = command.BranchId,
            IsActive = true
        };
        _branchRepository.GetByIdAsync(command.BranchId, default).Returns(branch);

        // For one product, we return null => means not found
        if (command.Items.Any())
        {
            _productRepository.GetByIdAsync(command.Items[0].ProductId, default).Returns((Product?)null);
        }

        // Act
        Func<Task> act = () => _handler.Handle(command, default);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage($"Product with ID '{command.Items[0].ProductId}' does not exist.");
    }

    /// <summary>
    /// Tests that the discount logic is correctly applied:
    /// - 4-9 items => 10%
    /// - 10-20 items => 20%
    /// - otherwise => 0
    /// </summary>
    [Fact(DisplayName = "Given discount rules When creating sale Then applies correct discounts and total")]
    public async Task Handle_DiscountRules_AppliesCorrectDiscountsAndTotal()
    {
        // Arrange
        var command = new CreateSaleCommand
        {
            BranchId = Guid.NewGuid(),
            Items = new List<CreateSaleItemCommand>
            {
                // 3 units => no discount
                new CreateSaleItemCommand { ProductId = Guid.NewGuid(), Quantity = 3 },
                // 5 units => 10% discount
                new CreateSaleItemCommand { ProductId = Guid.NewGuid(), Quantity = 5 },
                // 10 units => 20% discount
                new CreateSaleItemCommand { ProductId = Guid.NewGuid(), Quantity = 10 }
            }
        };

        var branch = new Branch { Name = "Test Branch", Id = command.BranchId, IsActive = true };
        _branchRepository.GetByIdAsync(command.BranchId, default).Returns(branch);

        var noDiscountProduct = new Product { Id = command.Items[0].ProductId, UnitPrice = 10m, IsActive = true };
        var tenPercentProduct = new Product { Id = command.Items[1].ProductId, UnitPrice = 20m, IsActive = true };
        var twentyPercentProduct = new Product { Id = command.Items[2].ProductId, UnitPrice = 50m, IsActive = true };

        _productRepository.GetByIdAsync(noDiscountProduct.Id, default).Returns(noDiscountProduct);
        _productRepository.GetByIdAsync(tenPercentProduct.Id, default).Returns(tenPercentProduct);
        _productRepository.GetByIdAsync(twentyPercentProduct.Id, default).Returns(twentyPercentProduct);

        var saleToReturn = new Sale
        {
            BranchId = command.BranchId,
            Items = [],
            SaleDate = DateTime.UtcNow,
            SaleNumber = Guid.NewGuid().ToString(),
            UserId = Guid.NewGuid(),
            Id = Guid.NewGuid()
        };
        _saleRepository.CreateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>()).Returns(saleToReturn);

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        await _saleRepository.Received(1).CreateAsync(
            Arg.Is<Sale>(sale =>
                sale.Items.Count == 3
                && CheckSaleItem(sale.Items[0], noDiscountProduct.Id, 3, 0m, 10m)
                && CheckSaleItem(sale.Items[1], tenPercentProduct.Id, 5, 0.1m, 20m)
                && CheckSaleItem(sale.Items[2], twentyPercentProduct.Id, 10, 0.2m, 50m)
                && CheckSaleTotal(sale)
            ),
            Arg.Any<CancellationToken>());

        result.Id.Should().Be(saleToReturn.Id);
    }

    private bool CheckSaleItem(SaleItem item, Guid productId, int quantity, decimal discount, decimal unitPrice)
    {
        if (item.ProductId != productId) return false;
        if (item.Quantity != quantity) return false;
        if (item.Discount != discount) return false;
        if (item.UnitPrice != unitPrice) return false;

        var rawTotal = unitPrice * quantity;
        var expectedTotal = rawTotal - rawTotal * discount;
        // small tolerance check for floating arithmetic if needed
        if (item.TotalItemAmount != expectedTotal) return false;

        return true;
    }

    private bool CheckSaleTotal(Sale sale)
    {
        // 1) 3 items of unitPrice=10 => rawTotal=30 => discount=0 => final=30
        // 2) 5 items of unitPrice=20 => rawTotal=100 => discount=10% => final=90
        // 3) 10 items of unitPrice=50 => rawTotal=500 => discount=20% => final=400
        // sum = 30 + 90 + 400 = 520
        return sale.TotalAmount == 520m;
    }

    private IHttpContextAccessor InitializeHttpContextAccessor()
    {
        var result = Substitute.For<IHttpContextAccessor>();

        var claims = new[]
        {
                new Claim(ClaimTypes.NameIdentifier, "11111111-1111-1111-1111-111111111111")
            };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var principal = new ClaimsPrincipal(identity);

        var httpContext = new DefaultHttpContext
        {
            User = principal
        };

        result.HttpContext.Returns(httpContext);

        return result;
    }
}
