using Ambev.DeveloperEvaluation.Application.Branches.CreateBranch;
using Ambev.DeveloperEvaluation.Application.Branches.DeleteBranch;
using Ambev.DeveloperEvaluation.Application.Branches.GetBranch;
using Ambev.DeveloperEvaluation.Unit.Presentation.TestData;
using Ambev.DeveloperEvaluation.WebApi.Common;
using Ambev.DeveloperEvaluation.WebApi.Features.Branches;
using Ambev.DeveloperEvaluation.WebApi.Features.Branches.CreateBranch;
using Ambev.DeveloperEvaluation.WebApi.Features.Branches.GetBranch;
using AutoMapper;
using FluentAssertions;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Presentation;

/// <summary>
/// Contains unit tests for the <see cref="BranchesController"/> class.
/// </summary>
public class BranchesControllerTests
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;
    private readonly BranchesController _controller;

    public BranchesControllerTests()
    {
        _mediator = Substitute.For<IMediator>();

        // create mapper with profiles
        var configuration = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<Ambev.DeveloperEvaluation.WebApi.Features.Branches.CreateBranch.CreateBranchProfile>();
            cfg.AddProfile<Ambev.DeveloperEvaluation.WebApi.Features.Branches.GetBranch.GetBranchProfile>();
        });
        _mapper = configuration.CreateMapper();
        _controller = new BranchesController(_mediator, _mapper);
    }

    #region CreateBranch

    [Fact(DisplayName = "CreateBranch with valid request returns 201 Created")]
    public async Task CreateBranch_ValidRequest_ReturnsCreated()
    {
        // Arrange
        var request = BranchesControllerTestData.GenerateValidCreateBranchRequest();
        var createBranchResult = new CreateBranchResult { Id = Guid.NewGuid(), Name = request.Name };

        _mediator.Send(Arg.Any<CreateBranchCommand>(), Arg.Any<CancellationToken>()).Returns(createBranchResult);

        // Act
        var actionResult = await _controller.CreateBranch(request, CancellationToken.None);

        // Assert
        var createdResult = actionResult as CreatedResult;
        createdResult.Should().NotBeNull();
        createdResult!.StatusCode.Should().Be(201);

        var apiResponse = createdResult.Value as ApiResponseWithData<CreateBranchResponse>;
        apiResponse.Should().NotBeNull();
        apiResponse!.Data!.Id.Should().Be(createBranchResult.Id);
        apiResponse.Data.Name.Should().Be(request.Name);
    }

    [Fact(DisplayName = "CreateBranch with invalid request returns 400 BadRequest")]
    public async Task CreateBranch_InvalidRequest_ReturnsBadRequest()
    {
        // Arrange
        var request = BranchesControllerTestData.GenerateInvalidCreateBranchRequest();

        // Act
        var actionResult = await _controller.CreateBranch(request, CancellationToken.None);

        // Assert
        var badRequest = actionResult as BadRequestObjectResult;
        badRequest.Should().NotBeNull();
        badRequest!.StatusCode.Should().Be(400);

        var errors = badRequest.Value as IEnumerable<object>;
        errors.Should().NotBeNull(); // The validation errors
    }

    #endregion

    #region GetBranch

    [Fact(DisplayName = "GetBranch with valid ID returns 200 OK")]
    public async Task GetBranch_ValidId_ReturnsOk()
    {
        // Arrange
        var branchId = Guid.NewGuid();
        var getBranchRequest = new GetBranchRequest { Id = branchId };
        var getBranchCommand = new GetBranchCommand(branchId);
        var getBranchResult = new GetBranchResult { Id = branchId, Name = "Test Branch", IsActive = true };
        var getBranchResponse = new GetBranchResponse { Id = branchId, Name = "Test Branch", IsActive = true };

        _mediator.Send(getBranchCommand, Arg.Any<CancellationToken>()).Returns(getBranchResult);

        // Act
        var actionResult = await _controller.GetBranch(branchId, CancellationToken.None);

        // Assert
        var okResult = actionResult as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);

        var apiResponse = okResult.Value as ApiResponseWithData<GetBranchResult>;
        apiResponse.Should().NotBeNull();
        apiResponse!.Data!.Id.Should().Be(branchId);
        apiResponse.Data.Name.Should().Be("Test Branch");
        apiResponse.Data.IsActive.Should().BeTrue();
    }

    [Fact(DisplayName = "GetBranch with invalid ID returns 400 BadRequest")]
    public async Task GetBranch_InvalidId_ReturnsBadRequest()
    {
        // ID = Guid.Empty => triggers request validation failure
        var actionResult = await _controller.GetBranch(Guid.Empty, CancellationToken.None);

        var badRequest = actionResult as BadRequestObjectResult;
        badRequest.Should().NotBeNull();
        badRequest!.StatusCode.Should().Be(400);
    }

    [Fact(DisplayName = "GetBranch not found throws KeyNotFoundException => 404 (handled by global filter)")]
    public async Task GetBranch_NotFound_ThrowsKeyNotFoundException()
    {
        var branchId = Guid.NewGuid();
        var getBranchCommand = new GetBranchCommand(branchId);

        _mediator
            .Send(getBranchCommand, Arg.Any<CancellationToken>())
            .Returns<GetBranchResult>(_ => throw new KeyNotFoundException($"Branch with ID {branchId} not found"));

        // Act
        Func<Task> act = () => _controller.GetBranch(branchId, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage($"Branch with ID {branchId} not found");
    }

    #endregion

    #region DeleteBranch

    [Fact(DisplayName = "DeleteBranch with valid ID returns 200 OK")]
    public async Task DeleteBranch_ValidId_ReturnsOk()
    {
        // Arrange
        var branchId = Guid.NewGuid();
        var deleteCommand = new DeleteBranchCommand(branchId);

        // Act
        var actionResult = await _controller.DeleteBranch(branchId, CancellationToken.None);

        // Assert
        var okResult = actionResult as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);

        var apiResponse = okResult.Value as ApiResponse;
        apiResponse.Should().NotBeNull();
        apiResponse!.Success.Should().BeTrue();
    }

    [Fact(DisplayName = "DeleteBranch with invalid ID returns 400 BadRequest")]
    public async Task DeleteBranch_InvalidId_ReturnsBadRequest()
    {
        // ID = Guid.Empty => triggers request validation failure
        var actionResult = await _controller.DeleteBranch(Guid.Empty, CancellationToken.None);

        var badRequest = actionResult as BadRequestObjectResult;
        badRequest.Should().NotBeNull();
        badRequest!.StatusCode.Should().Be(400);
    }

    #endregion
}
