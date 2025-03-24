using Ambev.DeveloperEvaluation.Application.Sales.DeleteSale;
using Ambev.DeveloperEvaluation.Domain.Events.Sales;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using FluentAssertions;
using MediatR;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Tests.Unit;

public class DeleteSaleHandlerTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMediator _mediator;
    private readonly DeleteSaleHandler _handler;

    public DeleteSaleHandlerTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _mediator = Substitute.For<IMediator>();
        _handler = new DeleteSaleHandler(_saleRepository, _mediator);
    }

    [Fact(DisplayName = "DeleteSaleHandler: valid command publishes SaleDeletedEvent")]
    public async Task Handle_ValidCommand_PublishesSaleDeletedEvent()
    {
        // Arrange
        var saleId = Guid.NewGuid();
        var command = new DeleteSaleCommand(saleId);
        _saleRepository.DeleteAsync(saleId, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(true));

        // Act
        var response = await _handler.Handle(command, CancellationToken.None);

        // Assert
        response.Should().NotBeNull();
        response.Success.Should().BeTrue();
        // Verify that the SaleDeletedEvent was published with the correct sale ID
        await _mediator.Received(1)
            .Publish(Arg.Is<SaleDeletedEvent>(e => e.SaleId == saleId), Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "DeleteSaleHandler: non-existent sale throws KeyNotFoundException")]
    public async Task Handle_NonExistentSale_ThrowsKeyNotFoundException()
    {
        // Arrange
        var saleId = Guid.NewGuid();
        var command = new DeleteSaleCommand(saleId);
        _saleRepository.DeleteAsync(saleId, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(false));

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(
            () => _handler.Handle(command, CancellationToken.None));
    }
}
