using Ambev.DeveloperEvaluation.Application.Sales.CancelSale;
using Ambev.DeveloperEvaluation.Domain.Events.Sales;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using FluentAssertions;
using MediatR;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales;

public class CancelSaleHandlerTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMediator _mediator;
    private readonly CancelSaleHandler _handler;

    public CancelSaleHandlerTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _mediator = Substitute.For<IMediator>();
        _handler = new CancelSaleHandler(_saleRepository, _mediator);
    }

    [Fact(DisplayName = "CancelSaleHandler: valid command publishes SaleCanceldEvent")]
    public async Task Handle_ValidCommand_PublishesSaleCanceldEvent()
    {
        // Arrange
        var saleId = Guid.NewGuid();
        var command = new CancelSaleCommand(saleId);
        _saleRepository.CancelAsync(saleId, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(true));

        // Act
        var response = await _handler.Handle(command, CancellationToken.None);

        // Assert
        response.Should().NotBeNull();
        response.Success.Should().BeTrue();
        // Verify that the SaleCanceldEvent was published with the correct sale ID
        await _mediator.Received(1)
            .Publish(Arg.Is<SaleCanceledEvent>(e => e.SaleId == saleId), Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "CancelSaleHandler: non-existent sale throws KeyNotFoundException")]
    public async Task Handle_NonExistentSale_ThrowsKeyNotFoundException()
    {
        // Arrange
        var saleId = Guid.NewGuid();
        var command = new CancelSaleCommand(saleId);
        _saleRepository.CancelAsync(saleId, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(false));

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(
            () => _handler.Handle(command, CancellationToken.None));
    }
}
