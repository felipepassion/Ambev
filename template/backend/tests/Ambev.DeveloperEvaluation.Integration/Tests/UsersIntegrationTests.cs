using Ambev.DeveloperEvaluation.Application;
using Ambev.DeveloperEvaluation.Application.Users.CreateUser;
using Ambev.DeveloperEvaluation.Application.Users.DeleteUser;
using Ambev.DeveloperEvaluation.Application.Users.GetUser;
using Ambev.DeveloperEvaluation.Common.Security;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.ORM;
using Ambev.DeveloperEvaluation.ORM.Repositories;
using Ambev.DeveloperEvaluation.Unit.Presentation.TestData;
using Ambev.DeveloperEvaluation.WebApi;
using Ambev.DeveloperEvaluation.WebApi.Common;
using Ambev.DeveloperEvaluation.WebApi.Features.Users;
using Ambev.DeveloperEvaluation.WebApi.Features.Users.CreateUser;
using Ambev.DeveloperEvaluation.WebApi.Features.Users.DeleteUser;
using Ambev.DeveloperEvaluation.WebApi.Features.Users.GetUser;
using AutoMapper;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Ambev.DeveloperEvaluation.Tests.Integration;

public class UsersIntegrationTests : IAsyncLifetime
{
    private readonly ServiceProvider _provider;
    private readonly DefaultContext _context;
    private readonly UsersController _controller;

    public UsersIntegrationTests()
    {
        var services = new ServiceCollection();
        services.AddDbContext<DefaultContext>(options =>
            options.UseInMemoryDatabase("TestUsersDb"));

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddSingleton<IPasswordHasher, BCryptPasswordHasher>();

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblies(
                typeof(ApplicationLayer).Assembly,
                typeof(Program).Assembly
            );
        });
        services.AddAutoMapper(cfg =>
        {
            cfg.AddProfile<Application.Users.CreateUser.CreateUserProfile>();
            cfg.AddProfile<Application.Users.GetUser.GetUserProfile>();
            
            cfg.AddProfile<WebApi.Features.Users.CreateUser.CreateUserProfile>();
            cfg.AddProfile<WebApi.Features.Users.GetUser.GetUserProfile>();
        });
        _provider = services.BuildServiceProvider();
        _context = _provider.GetRequiredService<DefaultContext>();
        var mediator = _provider.GetRequiredService<IMediator>();
        var mapper = _provider.GetRequiredService<IMapper>();
        _controller = new UsersController(mediator, mapper);
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public async Task DisposeAsync()
    {
        await _context.Database.EnsureDeletedAsync();
        _provider.Dispose();
    }

    [Fact(DisplayName = "CreateUser returns 201 and persists data")]
    public async Task CreateUser_ValidRequest_ReturnsCreated()
    {
        var request = UsersIntegrationTestData.GenerateValidCreateUserRequest();
        var actionResult = await _controller.CreateUser(request, CancellationToken.None);
        var createdResult = actionResult as CreatedResult;
        createdResult.Should().NotBeNull();
        createdResult.StatusCode.Should().Be(201);
        var apiResponse = createdResult.Value as ApiResponseWithData<CreateUserResponse>;
        apiResponse.Should().NotBeNull();
        
        apiResponse.Data!.Id.Should().NotBe(Guid.Empty);
        apiResponse.Data.Username.Should().Be(request.Username);
        var userFromDb = await _context.Users.FindAsync(apiResponse.Data.Id);
        userFromDb.Should().NotBeNull();
        userFromDb.Username.Should().Be(request.Username);
    }

    [Fact(DisplayName = "CreateUser with invalid request returns 400 BadRequest")]
    public async Task CreateUser_InvalidRequest_ReturnsBadRequest()
    {
        var request = UsersIntegrationTestData.GenerateInvalidCreateUserRequest();
        var actionResult = await _controller.CreateUser(request, CancellationToken.None);
        var badRequest = actionResult as BadRequestObjectResult;
        badRequest.Should().NotBeNull();
        badRequest.StatusCode.Should().Be(400);
        var errors = badRequest.Value as IEnumerable<object>;
        errors.Should().NotBeNull();
        errors.Should().NotBeEmpty();
    }

    [Fact(DisplayName = "GetUser returns 200 when user exists")]
    public async Task GetUser_ValidId_ReturnsOk()
    {
        var createReq = UsersIntegrationTestData.GenerateValidCreateUserRequest();
        var createActionResult = await _controller.CreateUser(createReq, CancellationToken.None);
        var createdResult = createActionResult as CreatedResult;
        createdResult.Should().NotBeNull();
        var createdApiResponse = createdResult.Value as ApiResponseWithData<CreateUserResponse>;
        var userId = createdApiResponse.Data.Id;
        var getActionResult = await _controller.GetUser(userId, CancellationToken.None);
        var okResult = getActionResult as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult.StatusCode.Should().Be(200);
        var getApiResponse = okResult.Value as ApiResponseWithData<GetUserResult>;
        getApiResponse.Should().NotBeNull();
        getApiResponse.Data.Id.Should().Be(userId);
        getApiResponse.Data.Username.Should().Be(createReq.Username);
        getApiResponse.Data.Email.Should().Be(createReq.Email);
        getApiResponse.Data.Phone.Should().Be(createReq.Phone);
    }

    [Fact(DisplayName = "GetUser with invalid ID returns 400 BadRequest")]
    public async Task GetUser_InvalidId_ReturnsBadRequest()
    {
        var actionResult = await _controller.GetUser(Guid.Empty, CancellationToken.None);
        var badRequest = actionResult as BadRequestObjectResult;
        badRequest.Should().NotBeNull();
        badRequest.StatusCode.Should().Be(400);
    }

    [Fact(DisplayName = "GetUser not found throws KeyNotFoundException")]
    public async Task GetUser_NotFound_ThrowsKeyNotFoundException()
    {
        var createReq = UsersIntegrationTestData.GenerateValidCreateUserRequest();
        var createActionResult = await _controller.CreateUser(createReq, CancellationToken.None);
        var createdResult = createActionResult as CreatedResult;
        createdResult.Should().NotBeNull();
        var createdApiResponse = createdResult.Value as ApiResponseWithData<CreateUserResponse>;
        var userId = createdApiResponse.Data.Id;
        var deleteResult = await _controller.DeleteUser(userId, CancellationToken.None);
        deleteResult.Should().BeOfType<OkObjectResult>();
        Func<Task> act = () => _controller.GetUser(userId, CancellationToken.None);
        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact(DisplayName = "DeleteUser returns 200 and subsequent Get throws KeyNotFoundException")]
    public async Task DeleteUser_ReturnsOkAndThenThrowsNotFound()
    {
        var createReq = UsersIntegrationTestData.GenerateValidCreateUserRequest();
        var createActionResult = await _controller.CreateUser(createReq, CancellationToken.None);
        var createdResult = createActionResult as CreatedResult;
        createdResult.Should().NotBeNull();
        var createdApiResponse = createdResult.Value as ApiResponseWithData<CreateUserResponse>;
        var userId = createdApiResponse.Data.Id;
        var deleteActionResult = await _controller.DeleteUser(userId, CancellationToken.None);
        var okDeleteResult = deleteActionResult as OkObjectResult;
        okDeleteResult.Should().NotBeNull();
        okDeleteResult.StatusCode.Should().Be(200);
        Func<Task> act = () => _controller.GetUser(userId, CancellationToken.None);
        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact(DisplayName = "DeleteUser with invalid ID returns 400 BadRequest")]
    public async Task DeleteUser_InvalidId_ReturnsBadRequest()
    {
        var actionResult = await _controller.DeleteUser(Guid.Empty, CancellationToken.None);
        var badRequest = actionResult as BadRequestObjectResult;
        badRequest.Should().NotBeNull();
        badRequest.StatusCode.Should().Be(400);
    }
}
