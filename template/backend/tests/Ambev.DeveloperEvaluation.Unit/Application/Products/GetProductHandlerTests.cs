using Ambev.DeveloperEvaluation.Application.Products.GetProduct;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Events.Products;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using FluentAssertions;
using MediatR;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Tests.Unit;

public class GetProductHandlerTests
{
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;
    private readonly IMediator _mediator;
    private readonly GetProductHandler _handler;

    public GetProductHandlerTests()
    {
        _productRepository = Substitute.For<IProductRepository>();
        _mapper = Substitute.For<IMapper>();
        _mediator = Substitute.For<IMediator>();
        _handler = new GetProductHandler(_productRepository, _mapper, _mediator);
    }

    [Fact(DisplayName = "GetProductHandler: valid command returns product details and publishes ProductRetrievedEvent")]
    public async Task Handle_ValidCommand_PublishesProductRetrievedEvent()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var product = new Product
        {
            Id = productId,
            Name = "Test Product",
            UnitPrice = 15m
        };
        var command = new GetProductCommand(productId);

        _productRepository.GetByIdAsync(productId, Arg.Any<CancellationToken>())!
            .Returns(Task.FromResult(product));

        var mappedResult = new GetProductResult
        {
            Id = productId,
            Name = product.Name,
            UnitPrice = product.UnitPrice
        };
        _mapper.Map<GetProductResult>(product).Returns(mappedResult);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(productId);

        // Verify that ProductRetrievedEvent is published with the correct product ID
        await _mediator.Received(1)
            .Publish(Arg.Is<ProductRetrievedEvent>(e => e.ProductId == productId), Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "GetProductHandler: non-existent product throws KeyNotFoundException")]
    public async Task Handle_NonExistentProduct_ThrowsKeyNotFoundException()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var command = new GetProductCommand(productId);

        _productRepository.GetByIdAsync(productId, Arg.Any<CancellationToken>())!
            .Returns(Task.FromResult<Product>(null!));

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(
            () => _handler.Handle(command, CancellationToken.None));
    }
}
