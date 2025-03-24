using Ambev.DeveloperEvaluation.Application.Sales.GetSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Events.Sales;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using FluentAssertions;
using MediatR;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Tests.Unit;

public class GetSaleHandlerTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly IMediator _mediator;
    private readonly GetSaleHandler _handler;

    public GetSaleHandlerTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _mapper = Substitute.For<IMapper>();
        _mediator = Substitute.For<IMediator>();
        _handler = new GetSaleHandler(_saleRepository, _mapper, _mediator);
    }

    [Fact(DisplayName = "GetSaleHandler: valid command returns sale details and publishes SaleRetrievedEvent")]
    public async Task Handle_ValidCommand_PublishesSaleRetrievedEvent()
    {
        // Arrange
        var saleId = Guid.NewGuid();
        var sale = new Sale
        {
            Id = saleId,
            BranchId = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            SaleDate = DateTime.UtcNow,
            IsCancelled = false,
            CreatedAt = DateTime.UtcNow,
            SaleNumber = "TestSaleNumber",
            Items = []
        };
        var command = new GetSaleCommand(saleId);

        _saleRepository.GetByIdAsync(saleId, Arg.Any<CancellationToken>())!
            .Returns(Task.FromResult(sale));

        var mappedResult = new GetSaleResult { Id = saleId };
        _mapper.Map<GetSaleResult>(sale).Returns(mappedResult);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(saleId);
        // Verify that the SaleRetrievedEvent was published with the correct sale ID
        await _mediator.Received(1)
            .Publish(Arg.Is<SaleRetrievedEvent>(e => e.SaleId == saleId), Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "GetSaleHandler: non-existent sale throws KeyNotFoundException")]
    public async Task Handle_NonExistentSale_ThrowsKeyNotFoundException()
    {
        // Arrange
        var saleId = Guid.NewGuid();
        var command = new GetSaleCommand(saleId);

        _saleRepository.GetByIdAsync(saleId, Arg.Any<CancellationToken>())!
            .Returns(Task.FromResult<Sale>(null!));

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(
            () => _handler.Handle(command, CancellationToken.None));
    }
}
