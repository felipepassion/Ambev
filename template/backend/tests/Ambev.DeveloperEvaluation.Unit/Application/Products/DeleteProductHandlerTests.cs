using Ambev.DeveloperEvaluation.Application.Products.DeleteProduct;
using Ambev.DeveloperEvaluation.Domain.Events.Products;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using FluentAssertions;
using MediatR;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Products;

public class DeleteProductHandlerTests
{
    private readonly IProductRepository _productRepository;
    private readonly IMediator _mediator;
    private readonly DeleteProductHandler _handler;

    public DeleteProductHandlerTests()
    {
        _productRepository = Substitute.For<IProductRepository>();
        _mediator = Substitute.For<IMediator>();
        _handler = new DeleteProductHandler(_productRepository, _mediator);
    }

    [Fact(DisplayName = "DeleteProductHandler: valid command publishes ProductDeletedEvent")]
    public async Task Handle_ValidCommand_PublishesProductDeletedEvent()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var command = new DeleteProductCommand(productId);
        _productRepository.DeleteAsync(productId, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(true));

        // Act
        var response = await _handler.Handle(command, CancellationToken.None);

        // Assert
        response.Should().NotBeNull();
        response.Success.Should().BeTrue();

        // Verify that ProductDeletedEvent is published with the correct product ID
        await _mediator.Received(1)
            .Publish(Arg.Is<ProductDeletedEvent>(e => e.ProductId == productId), Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "DeleteProductHandler: non-existent product throws KeyNotFoundException")]
    public async Task Handle_NonExistentProduct_ThrowsKeyNotFoundException()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var command = new DeleteProductCommand(productId);
        _productRepository.DeleteAsync(productId, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(false));

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(
            () => _handler.Handle(command, CancellationToken.None));
    }
}
