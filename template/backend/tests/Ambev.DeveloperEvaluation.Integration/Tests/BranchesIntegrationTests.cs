using Ambev.DeveloperEvaluation.Application;
using Ambev.DeveloperEvaluation.Application.Branches.GetBranch;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.ORM;
using Ambev.DeveloperEvaluation.ORM.Repositories;
using Ambev.DeveloperEvaluation.Unit.Presentation.TestData;
using Ambev.DeveloperEvaluation.WebApi;
using Ambev.DeveloperEvaluation.WebApi.Common;
using Ambev.DeveloperEvaluation.WebApi.Features.Branches;
using Ambev.DeveloperEvaluation.WebApi.Features.Branches.CreateBranch;
using AutoMapper;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Ambev.DeveloperEvaluation.Tests.Integration
{
    public class BranchesIntegrationTests : IAsyncLifetime
    {
        private readonly ServiceProvider _provider;
        private readonly DefaultContext _context;
        private readonly BranchesController _controller;

        public BranchesIntegrationTests()
        {
            var services = new ServiceCollection();

            // Use an in-memory DB for testing
            services.AddDbContext<DefaultContext>(options =>
                options.UseInMemoryDatabase("TestBranchesDb"));

            services.AddScoped<IBranchRepository, BranchRepository>();

            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssemblies(
                    typeof(ApplicationLayer).Assembly,
                    typeof(Program).Assembly
                );
            });

            services.AddAutoMapper(cfg =>
            {
                cfg.AddProfile<GetBranchProfile>();
                cfg.AddProfile<CreateBranchProfile>();
                cfg.AddProfile<Application.Branches.CreateBranch.CreateBranchProfile>();
                cfg.AddProfile<WebApi.Features.Branches.GetBranch.GetBranchProfile>();
            });

            _provider = services.BuildServiceProvider();

            _context = _provider.GetRequiredService<DefaultContext>();

            var mediator = _provider.GetRequiredService<IMediator>();
            var mapper = _provider.GetRequiredService<IMapper>();
            _controller = new BranchesController(mediator, mapper);
        }

        public Task InitializeAsync() => Task.CompletedTask;

        public async Task DisposeAsync()
        {
            await _context.Database.EnsureDeletedAsync();
            _provider.Dispose();
        }

        #region CreateBranch

        [Fact(DisplayName = "CreateBranch returns 201 and persists data")]
        public async Task CreateBranch_ValidRequest_ReturnsCreated()
        {
            // Arrange
            var request = BranchesIntegrationTestData.GenerateValidCreateBranchRequest();

            // Act
            var actionResult = await _controller.CreateBranch(request, CancellationToken.None);

            // Assert
            var createdResult = actionResult as CreatedResult;
            createdResult.Should().NotBeNull();
            createdResult!.StatusCode.Should().Be(201);

            var apiResponse = createdResult.Value as ApiResponseWithData<CreateBranchResponse>;
            apiResponse.Should().NotBeNull();
            apiResponse!.Data!.Id.Should().NotBe(Guid.Empty);
            apiResponse.Data.Name.Should().Be(request.Name);

            // Verify from the database
            var branchFromDb = await _context.Branches.FindAsync(apiResponse.Data!.Id);
            branchFromDb.Should().NotBeNull();
            branchFromDb!.Name.Should().Be(request.Name);
        }

        [Fact(DisplayName = "CreateBranch with invalid request returns 400 BadRequest")]
        public async Task CreateBranch_InvalidRequest_ReturnsBadRequest()
        {
            // Arrange
            var request = BranchesIntegrationTestData.GenerateInvalidCreateBranchRequest();

            // Act
            var actionResult = await _controller.CreateBranch(request, CancellationToken.None);

            // Assert
            var badRequest = actionResult as BadRequestObjectResult;
            badRequest.Should().NotBeNull();
            badRequest!.StatusCode.Should().Be(400);

            var errors = badRequest.Value as IEnumerable<object>;
            errors.Should().NotBeNull();
            errors!.Should().NotBeEmpty();
        }

        #endregion

        #region GetBranch

        [Fact(DisplayName = "GetBranch returns 200 when branch exists")]
        public async Task GetBranch_ValidId_ReturnsOk()
        {
            // Arrange: create a branch
            var createRequest = BranchesIntegrationTestData.GenerateValidCreateBranchRequest();
            var createActionResult = await _controller.CreateBranch(createRequest, CancellationToken.None);
            var createdResult = createActionResult as CreatedResult;
            createdResult.Should().NotBeNull();

            var createdApiResponse = createdResult.Value as ApiResponseWithData<CreateBranchResponse>;
            var branchId = createdApiResponse!.Data!.Id;

            // Act: call GetBranch
            var getActionResult = await _controller.GetBranch(branchId, CancellationToken.None);

            // Assert
            var okResult = getActionResult as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult!.StatusCode.Should().Be(200);

            var getApiResponse = okResult.Value as ApiResponseWithData<GetBranchResult>;
            getApiResponse.Should().NotBeNull();
            getApiResponse!.Data!.Id.Should().Be(branchId);
            getApiResponse.Data.Name.Should().Be(createRequest.Name);
            getApiResponse.Data.IsActive.Should().BeTrue();
        }

        [Fact(DisplayName = "GetBranch with invalid ID returns 400 BadRequest")]
        public async Task GetBranch_InvalidId_ReturnsBadRequest()
        {
            // Act
            var actionResult = await _controller.GetBranch(Guid.Empty, CancellationToken.None);

            // Assert
            var badRequest = actionResult as BadRequestObjectResult;
            badRequest.Should().NotBeNull();
            badRequest!.StatusCode.Should().Be(400);
        }

        [Fact(DisplayName = "GetBranch not found throws KeyNotFoundException")]
        public async Task GetBranch_NotFound_ThrowsKeyNotFoundException()
        {
            // Arrange: create & delete the branch to simulate not found
            var createRequest = BranchesIntegrationTestData.GenerateValidCreateBranchRequest();
            var createActionResult = await _controller.CreateBranch(createRequest, CancellationToken.None);
            var createdResult = createActionResult as CreatedResult;
            createdResult.Should().NotBeNull();

            var createdApiResponse = createdResult.Value as ApiResponseWithData<CreateBranchResponse>;
            var branchId = createdApiResponse!.Data!.Id;

            // Delete the branch
            var deleteResult = await _controller.DeleteBranch(branchId, CancellationToken.None);
            deleteResult.Should().BeOfType<OkObjectResult>();

            // Act
            Func<Task> act = () => _controller.GetBranch(branchId, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<KeyNotFoundException>();
        }

        #endregion

        #region DeleteBranch

        [Fact(DisplayName = "DeleteBranch returns 200 and subsequent Get throws KeyNotFoundException")]
        public async Task DeleteBranch_ReturnsOkAndThenThrowsNotFound()
        {
            // Arrange: create a branch
            var createRequest = BranchesIntegrationTestData.GenerateValidCreateBranchRequest();
            var createActionResult = await _controller.CreateBranch(createRequest, CancellationToken.None);
            var createdResult = createActionResult as CreatedResult;
            createdResult.Should().NotBeNull();

            var createdApiResponse = createdResult.Value as ApiResponseWithData<CreateBranchResponse>;
            var branchId = createdApiResponse!.Data!.Id;

            var deleteActionResult = await _controller.DeleteBranch(branchId, CancellationToken.None);
            var okDeleteResult = deleteActionResult as OkObjectResult;
            okDeleteResult.Should().NotBeNull();
            okDeleteResult!.StatusCode.Should().Be(200);

            Func<Task> act = () => _controller.GetBranch(branchId, CancellationToken.None);
            await act.Should().ThrowAsync<KeyNotFoundException>();
        }

        [Fact(DisplayName = "DeleteBranch with invalid ID returns 400 BadRequest")]
        public async Task DeleteBranch_InvalidId_ReturnsBadRequest()
        {
            // Act
            var actionResult = await _controller.DeleteBranch(Guid.Empty, CancellationToken.None);

            // Assert
            var badRequest = actionResult as BadRequestObjectResult;
            badRequest.Should().NotBeNull();
            badRequest!.StatusCode.Should().Be(400);
        }

        #endregion
    }
}
