using Ambev.DeveloperEvaluation.Application.Products.CreateProduct;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Events.Products;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Unit.Application._TestData;
using AutoMapper;
using FluentAssertions;
using FluentValidation;
using MediatR;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Products;

/// <summary>
/// Contains unit tests for the <see cref="CreateProductHandler"/> class.
/// </summary>
public class CreateProductHandlerTests
{
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;
    private readonly CreateProductHandler _handler;
    private readonly IMediator _mediator;

    public CreateProductHandlerTests()
    {
        _productRepository = Substitute.For<IProductRepository>();
        _mapper = Substitute.For<IMapper>();
        _mediator = Substitute.For<IMediator>();

        _handler = new CreateProductHandler(_mapper, _mediator, _productRepository);
    }

    [Fact(DisplayName = "Given valid product data When creating product Then returns success response")]
    public async Task Handle_ValidRequest_ReturnsSuccessResponse()
    {
        // Arrange
        var command = CreateProductHandlerTestData.GenerateValidCommand();
        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = command.Name,
            Description = command.Description,
            UnitPrice = command.UnitPrice,
            IsActive = true
        };
        var result = new CreateProductResult { Id = product.Id };

        _mapper.Map<Product>(command).Returns(product);
        _mapper.Map<CreateProductResult>(product).Returns(result);
        _productRepository.CreateAsync(product, default).Returns(product);

        // Act
        var response = await _handler.Handle(command, default);

        // Assert
        response.Should().NotBeNull();
        response.Id.Should().Be(product.Id);
        await _productRepository.Received(1).CreateAsync(Arg.Any<Product>(), Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Given invalid product data When creating product Then throws validation exception")]
    public async Task Handle_InvalidRequest_ThrowsValidationException()
    {
        // Arrange
        var command = CreateProductHandlerTestData.GenerateInvalidCommand();

        // Act
        var act = () => _handler.Handle(command, default);

        // Assert
        await act.Should().ThrowAsync<ValidationException>();
    }

    [Fact(DisplayName = "Given valid command When handling Then maps command to product entity")]
    public async Task Handle_ValidRequest_MapsCommandToProduct()
    {
        // Arrange
        var command = CreateProductHandlerTestData.GenerateValidCommand();
        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = command.Name,
            Description = command.Description,
            UnitPrice = command.UnitPrice
        };

        _mapper.Map<Product>(command).Returns(product);
        _productRepository.CreateAsync(product, default).Returns(product);

        // Act
        await _handler.Handle(command, default);

        // Assert
        _mapper.Received(1).Map<Product>(Arg.Is<CreateProductCommand>(c =>
            c.Name == command.Name &&
            c.Description == command.Description &&
            c.UnitPrice == command.UnitPrice));
    }

    [Fact(DisplayName = "CreateProductHandler: valid command returns created product ID and publishes ProductCreatedEvent")]
    public async Task Handle_ValidCommand_PublishesProductCreatedEvent()
    {
        // Arrange
        var command = new CreateProductCommand
        {
            Name = "Test Product",
            Description = "Test Description",
            UnitPrice = 100m
        };

        var productEntity = new Product
        {
            Id = Guid.NewGuid(),
            Name = command.Name,
            Description = command.Description,
            UnitPrice = command.UnitPrice
        };
        var productResult = new CreateProductResult { Id = productEntity.Id };

        _mapper.Map<Product>(command).Returns(productEntity);
        _productRepository.CreateAsync(Arg.Any<Product>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(productEntity));
        _mapper.Map<CreateProductResult>(productEntity).Returns(productResult);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(productEntity.Id);
        // Verify that the ProductCreatedEvent is published with the correct product details
        await _mediator.Received(1)
            .Publish(Arg.Is<ProductCreatedEvent>(e =>
                e.ProductId == productEntity.Id &&
                e.Name == productEntity.Name &&
                e.Description == productEntity.Description &&
                e.UnitPrice == productEntity.UnitPrice
            ), Arg.Any<CancellationToken>());
    }
}
