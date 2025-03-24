using Ambev.DeveloperEvaluation.Application.SaleItems.GetSaleItem;
using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.Application.Sales.DeleteSale;
using Ambev.DeveloperEvaluation.Application.Sales.GetSale;
using Ambev.DeveloperEvaluation.Unit.Presentation.TestData;
using Ambev.DeveloperEvaluation.WebApi.Common;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.GetSale;
using AutoMapper;
using FluentAssertions;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Presentation;

/// <summary>
/// Contains unit tests for the <see cref="SalesController"/> class.
/// </summary>
public class SalesControllerTests
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;
    private readonly SalesController _controller;

    public SalesControllerTests()
    {
        _mediator = Substitute.For<IMediator>();

        var configuration = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<WebApi.Features.Sales.CreateSale.CreateSaleProfile>();
            cfg.AddProfile<WebApi.Features.Sales.GetSale.GetSaleProfile>();
        });
        _mapper = configuration.CreateMapper();

        _controller = new SalesController(_mediator, _mapper);
    }

    #region CreateSale

    [Fact(DisplayName = "CreateSale with valid request returns 201 Created")]
    public async Task CreateSale_ValidRequest_ReturnsCreated()
    {
        // Arrange
        var request = SalesControllerTestData.GenerateValidCreateSaleRequest();
        var createSaleCommand = new CreateSaleCommand
        {
            BranchId = request.BranchId,
            Items = request.Items.Select(i => new CreateSaleItemCommand
            {
                ProductId = i.ProductId,
                Quantity = i.Quantity
            }).ToList()
        };
        var createSaleResult = new CreateSaleResult
        {
            Id = Guid.NewGuid(),
            TotalAmount = 100m
        };

        _mediator.Send(Arg.Any<CreateSaleCommand>(), Arg.Any<CancellationToken>()).Returns(createSaleResult);

        // Act
        var actionResult = await _controller.CreateSale(request, CancellationToken.None);

        // Assert
        var createdResult = actionResult as CreatedResult;
        createdResult.Should().NotBeNull();
        createdResult!.StatusCode.Should().Be(201);

        var apiResponse = createdResult.Value as ApiResponseWithData<CreateSaleResponse>;
        apiResponse.Should().NotBeNull();
        apiResponse!.Data!.Id.Should().Be(createSaleResult.Id);
    }

    [Fact(DisplayName = "CreateSale with invalid request returns 400 BadRequest")]
    public async Task CreateSale_InvalidRequest_ReturnsBadRequest()
    {
        // Arrange
        var request = SalesControllerTestData.GenerateInvalidCreateSaleRequest();

        // Act
        var actionResult = await _controller.CreateSale(request, CancellationToken.None);

        // Assert
        var badRequest = actionResult as BadRequestObjectResult;
        badRequest.Should().NotBeNull();
        badRequest!.StatusCode.Should().Be(400);

        var errors = badRequest.Value as IEnumerable<object>;
        errors.Should().NotBeNull(); // The validation errors
    }

    #endregion

    #region GetSale

    [Fact(DisplayName = "GetSale with valid ID returns 200 OK")]
    public async Task GetSale_ValidId_ReturnsOk()
    {
        // Arrange
        var saleId = Guid.NewGuid();
        var getSaleCommand = new GetSaleCommand(saleId);
        var getSaleResult = new GetSaleResult
        {
            Id = saleId,
            TotalAmount = 200m,
            IsCancelled = false,
            Items = new List<GetSaleItemResult>
            {
                new GetSaleItemResult
                {
                    ProductId = Guid.NewGuid(),
                    Quantity = 5,
                    Discount = 0.1m,
                    TotalItemAmount = 45m
                }
            }
        };
        var getSaleResponse = new GetSaleResponse
        {
            Id = saleId,
            TotalAmount = 200m,
            IsCancelled = false,
            Items = new List<GetSaleItemResponse>
            {
                new GetSaleItemResponse
                {
                    ProductId = getSaleResult.Items[0].ProductId,
                    Quantity = 5,
                    Discount = 0.1m,
                    TotalItemAmount = 45m
                }
            }
        };

        _mediator.Send(getSaleCommand, Arg.Any<CancellationToken>()).Returns(getSaleResult);

        // Act
        var actionResult = await _controller.GetSale(saleId, CancellationToken.None);

        // Assert
        var okResult = actionResult as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);

        var apiResponse = okResult.Value as ApiResponseWithData<GetSaleResponse>;
        apiResponse.Should().NotBeNull();
        apiResponse!.Data!.Id.Should().Be(saleId);
        apiResponse.Data.Items.Should().HaveCount(1);
        apiResponse.Data.TotalAmount.Should().Be(200m);
    }

    [Fact(DisplayName = "GetSale with invalid ID returns 400 BadRequest")]
    public async Task GetSale_InvalidId_ReturnsBadRequest()
    {
        // ID = Guid.Empty => triggers request validation failure
        var actionResult = await _controller.GetSale(Guid.Empty, CancellationToken.None);

        var badRequest = actionResult as BadRequestObjectResult;
        badRequest.Should().NotBeNull();
        badRequest!.StatusCode.Should().Be(400);
    }

    [Fact(DisplayName = "GetSale not found throws KeyNotFoundException => 404 (with global filter)")]
    public async Task GetSale_NotFound_ThrowsKeyNotFoundException()
    {
        var saleId = Guid.NewGuid();
        var getSaleCommand = new GetSaleCommand(saleId);

        _mediator.Send(getSaleCommand, Arg.Any<CancellationToken>())
                 .Returns<GetSaleResult>(_ => throw new KeyNotFoundException($"Sale with ID {saleId} not found"));

        // Act
        Func<Task> act = () => _controller.GetSale(saleId, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage($"Sale with ID {saleId} not found");
    }

    #endregion

    #region DeleteSale

    [Fact(DisplayName = "DeleteSale with valid ID returns 200 OK")]
    public async Task DeleteSale_ValidId_ReturnsOk()
    {
        // Arrange
        var saleId = Guid.NewGuid();
        var deleteCommand = new DeleteSaleCommand(saleId);

        // Act
        var actionResult = await _controller.DeleteSale(saleId, CancellationToken.None);

        // Assert
        var okResult = actionResult as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);

        var apiResponse = okResult.Value as ApiResponse;
        apiResponse.Should().NotBeNull();
        apiResponse!.Success.Should().BeTrue();
    }

    [Fact(DisplayName = "DeleteSale with invalid ID returns 400 BadRequest")]
    public async Task DeleteSale_InvalidId_ReturnsBadRequest()
    {
        // ID = Guid.Empty => triggers request validation failure
        var actionResult = await _controller.DeleteSale(Guid.Empty, CancellationToken.None);

        var badRequest = actionResult as BadRequestObjectResult;
        badRequest.Should().NotBeNull();
        badRequest!.StatusCode.Should().Be(400);
    }

    #endregion
}
