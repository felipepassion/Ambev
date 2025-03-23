using Ambev.DeveloperEvaluation.Application.Branches.CreateBranch;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Unit.Application.TestData;
using AutoMapper;
using FluentAssertions;
using FluentValidation;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application;

/// <summary>
/// Contains unit tests for the <see cref="CreateBranchHandler"/> class.
/// </summary>
public class CreateBranchHandlerTests
{
    private readonly IBranchRepository _branchRepository;
    private readonly IMapper _mapper;
    private readonly CreateBranchHandler _handler;

    /// <summary>
    /// Initializes a new instance of the <see cref="CreateBranchHandlerTests"/> class.
    /// Sets up the test dependencies.
    /// </summary>
    public CreateBranchHandlerTests()
    {
        _branchRepository = Substitute.For<IBranchRepository>();
        _mapper = Substitute.For<IMapper>();
        _handler = new CreateBranchHandler(_branchRepository, _mapper);
    }

    /// <summary>
    /// Tests that a valid branch creation request is handled successfully.
    /// </summary>
    [Fact(DisplayName = "Given valid branch data When creating branch Then returns success response")]
    public async Task Handle_ValidRequest_ReturnsSuccessResponse()
    {
        // Arrange
        var command = CreateBranchHandlerTestData.GenerateValidCommand();
        var branch = new Branch
        {
            Id = Guid.NewGuid(),
            Name = command.Name,
            IsActive = true
        };
        var result = new CreateBranchResult
        {
            Id = branch.Id
        };

        _mapper.Map<Branch>(command).Returns(branch);
        _mapper.Map<CreateBranchResult>(branch).Returns(result);

        _branchRepository.CreateAsync(branch, default).ReturnsForAnyArgs(branch);

        // Act
        var createBranchResult = await _handler.Handle(command, default);

        // Assert
        createBranchResult.Should().NotBeNull();
        createBranchResult.Id.Should().Be(branch.Id);

        await _branchRepository.Received(1).CreateAsync(Arg.Any<Branch>(), Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Tests that an invalid branch creation request throws a validation exception.
    /// </summary>
    [Fact(DisplayName = "Given invalid branch data When creating branch Then throws validation exception")]
    public async Task Handle_InvalidRequest_ThrowsValidationException()
    {
        // Arrange
        var invalidCommand = CreateBranchHandlerTestData.GenerateInvalidCommand();

        // Act
        Func<Task> act = () => _handler.Handle(invalidCommand, default);

        // Assert
        await act.Should().ThrowAsync<ValidationException>();
    }

    /// <summary>
    /// Tests that the mapper is called with the correct command.
    /// </summary>
    [Fact(DisplayName = "Given valid command When handling Then maps command to branch entity")]
    public async Task Handle_ValidRequest_MapsCommandToBranch()
    {
        // Arrange
        var command = CreateBranchHandlerTestData.GenerateValidCommand();
        var branch = new Branch
        {
            Id = Guid.NewGuid(),
            Name = command.Name
        };

        _mapper.Map<Branch>(command).Returns(branch);
        _branchRepository.CreateAsync(branch, default).ReturnsForAnyArgs(branch);

        // Act
        await _handler.Handle(command, default);

        // Assert
        _mapper.Received(1).Map<Branch>(Arg.Is<CreateBranchCommand>(cmd =>
            cmd.Name == command.Name
        ));
    }
}
