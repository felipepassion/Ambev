using Ambev.DeveloperEvaluation.Application.Products.CreateProduct;
using Ambev.DeveloperEvaluation.Application.Products.DeleteProduct;
using Ambev.DeveloperEvaluation.Application.Products.GetProduct;
using Ambev.DeveloperEvaluation.Unit.Presentation.TestData;
using Ambev.DeveloperEvaluation.WebApi.Common;
using Ambev.DeveloperEvaluation.WebApi.Features.Products;
using Ambev.DeveloperEvaluation.WebApi.Features.Products.CreateProduct;
using Ambev.DeveloperEvaluation.WebApi.Features.Products.GetProduct;
using AutoMapper;
using FluentAssertions;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Presentation;

/// <summary>
/// Contains unit tests for the <see cref="ProductsController"/> class.
/// </summary>
public class ProductsControllerTests
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;
    private readonly ProductsController _controller;

    public ProductsControllerTests()
    {
        _mediator = Substitute.For<IMediator>();

        var configuration = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<WebApi.Features.Products.CreateProduct.CreateProductProfile>();
            cfg.AddProfile<WebApi.Features.Products.GetProduct.GetProductProfile>();
        });
        _mapper = configuration.CreateMapper();

        _controller = new ProductsController(_mediator, _mapper);
    }

    #region CreateProduct

    [Fact(DisplayName = "CreateProduct with valid request returns 201 Created")]
    public async Task CreateProduct_ValidRequest_ReturnsCreated()
    {
        // Arrange
        var request = ProductsControllerTestData.GenerateValidCreateProductRequest();
        var createProductCommand = new CreateProductCommand
        {
            Name = request.Name,
            Description = request.Description,
            UnitPrice = request.UnitPrice
        };
        var createProductResult = new CreateProductResult
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Description = request.Description,
            UnitPrice = request.UnitPrice
        };

        // Mock do mapeamento
        _mediator.Send(Arg.Any<CreateProductCommand>(), Arg.Any<CancellationToken>()).Returns(createProductResult);

        // Act
        var actionResult = await _controller.CreateProduct(request, CancellationToken.None);

        // Assert
        var createdResult = actionResult as CreatedResult;
        createdResult.Should().NotBeNull();
        createdResult!.StatusCode.Should().Be(201);

        var apiResponse = createdResult.Value as ApiResponseWithData<CreateProductResponse>;
        apiResponse.Should().NotBeNull();
        apiResponse!.Data!.Id.Should().Be(createProductResult.Id);
        apiResponse.Data.Name.Should().Be(request.Name);
        apiResponse.Data.Description.Should().Be(request.Description);
        apiResponse.Data.UnitPrice.Should().Be(request.UnitPrice);
    }

    [Fact(DisplayName = "CreateProduct with invalid request returns 400 BadRequest")]
    public async Task CreateProduct_InvalidRequest_ReturnsBadRequest()
    {
        // Arrange
        var request = ProductsControllerTestData.GenerateInvalidCreateProductRequest();

        // Act
        var actionResult = await _controller.CreateProduct(request, CancellationToken.None);

        // Assert
        var badRequest = actionResult as BadRequestObjectResult;
        badRequest.Should().NotBeNull();
        badRequest!.StatusCode.Should().Be(400);

        var errors = badRequest.Value as IEnumerable<object>;
        errors.Should().NotBeNull(); // The validation errors
    }

    #endregion

    #region GetProduct

    [Fact(DisplayName = "GetProduct with valid ID returns 200 OK")]
    public async Task GetProduct_ValidId_ReturnsOk()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var getProductCommand = new GetProductCommand(productId);
        var getProductResult = new GetProductResult
        {
            Id = productId,
            Name = "Test Product",
            Description = "Test Desc",
            UnitPrice = 99.99m,
            IsActive = true
        };
        var getProductResponse = new GetProductResponse
        {
            Id = productId,
            Name = "Test Product",
            Description = "Test Desc",
            UnitPrice = 99.99m,
            IsActive = true
        };

        _mediator.Send(Arg.Any<GetProductCommand>(), Arg.Any<CancellationToken>()).Returns(getProductResult);

        // Act
        var actionResult = await _controller.GetProduct(productId, CancellationToken.None);

        // Assert
        var okResult = actionResult as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);

        var apiResponse = okResult.Value as ApiResponseWithData<GetProductResult>;
        apiResponse.Should().NotBeNull();
        apiResponse!.Data!.Id.Should().Be(productId);
        apiResponse.Data.Name.Should().Be("Test Product");
        apiResponse.Data.IsActive.Should().BeTrue();
    }

    [Fact(DisplayName = "GetProduct with invalid ID returns 400 BadRequest")]
    public async Task GetProduct_InvalidId_ReturnsBadRequest()
    {
        var actionResult = await _controller.GetProduct(Guid.Empty, CancellationToken.None);

        var badRequest = actionResult as BadRequestObjectResult;
        badRequest.Should().NotBeNull();
        badRequest!.StatusCode.Should().Be(400);
    }

    [Fact(DisplayName = "GetProduct not found throws KeyNotFoundException => 404 (with global filter)")]
    public async Task GetProduct_NotFound_ThrowsKeyNotFoundException()
    {
        var productId = Guid.NewGuid();
        var getProductCommand = new GetProductCommand(productId);

        _mediator.Send(getProductCommand, Arg.Any<CancellationToken>())
                 .Returns<GetProductResult>(_ => throw new KeyNotFoundException($"Product with ID {productId} not found"));

        // Act
        Func<Task> act = () => _controller.GetProduct(productId, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage($"Product with ID {productId} not found");
    }

    #endregion

    #region DeleteProduct

    [Fact(DisplayName = "DeleteProduct with valid ID returns 200 OK")]
    public async Task DeleteProduct_ValidId_ReturnsOk()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var deleteCommand = new DeleteProductCommand(productId);

        // Act
        var actionResult = await _controller.DeleteProduct(productId, CancellationToken.None);

        // Assert
        var okResult = actionResult as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);

        var apiResponse = okResult.Value as ApiResponse;
        apiResponse.Should().NotBeNull();
        apiResponse!.Success.Should().BeTrue();
    }

    [Fact(DisplayName = "DeleteProduct with invalid ID returns 400 BadRequest")]
    public async Task DeleteProduct_InvalidId_ReturnsBadRequest()
    {
        // ID = Guid.Empty => triggers request validation failure
        var actionResult = await _controller.DeleteProduct(Guid.Empty, CancellationToken.None);

        var badRequest = actionResult as BadRequestObjectResult;
        badRequest.Should().NotBeNull();
        badRequest!.StatusCode.Should().Be(400);
    }

    #endregion
}
