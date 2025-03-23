using Ambev.DeveloperEvaluation.Application.Users.CreateUser;
using Ambev.DeveloperEvaluation.Application.Users.DeleteUser;
using Ambev.DeveloperEvaluation.Application.Users.GetUser;
using Ambev.DeveloperEvaluation.Unit.Presentation.TestData;
using Ambev.DeveloperEvaluation.WebApi.Common;
using Ambev.DeveloperEvaluation.WebApi.Features.Users;
using Ambev.DeveloperEvaluation.WebApi.Features.Users.CreateUser;
using Ambev.DeveloperEvaluation.WebApi.Features.Users.GetUser;
using AutoMapper;
using FluentAssertions;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Presentation
{
    /// <summary>
    /// Contains unit tests for the <see cref="UsersController"/> class.
    /// </summary>
    public class UsersControllerTests
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        private readonly UsersController _controller;

        public UsersControllerTests()
        {
            _mediator = Substitute.For<IMediator>();
            // create mapper with profiles
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<Ambev.DeveloperEvaluation.WebApi.Features.Users.CreateUser.CreateUserProfile>();
                cfg.AddProfile<Ambev.DeveloperEvaluation.WebApi.Features.Users.GetUser.GetUserProfile>();
            });
            _mapper = configuration.CreateMapper();
            _controller = new UsersController(_mediator, _mapper);
        }

        #region CreateUser

        [Fact(DisplayName = "CreateUser with valid request returns 201 Created")]
        public async Task CreateUser_ValidRequest_ReturnsCreated()
        {
            // Arrange
            var request = UsersControllerTestData.GenerateValidCreateUserRequest();
            var createUserCommand = new CreateUserCommand
            {
                Username = request.Username,
                Password = request.Password,
                Email = request.Email,
                Phone = request.Phone,
                Status = request.Status,
                Role = request.Role
            };

            var createUserResult = new CreateUserResult { Id = Guid.NewGuid(), };
            var responseDto = new CreateUserResponse
            {
                Id = createUserResult.Id,
            };

            _mediator.Send(Arg.Any<CreateUserCommand>(), Arg.Any<CancellationToken>()).Returns(createUserResult);

            // Act
            var actionResult = await _controller.CreateUser(request, CancellationToken.None);

            // Assert
            var createdResult = actionResult as CreatedResult;
            createdResult.Should().NotBeNull();
            createdResult!.StatusCode.Should().Be(201);

            var apiResponse = createdResult.Value as ApiResponseWithData<CreateUserResponse>;
            apiResponse.Should().NotBeNull();
            apiResponse!.Data!.Id.Should().Be(responseDto.Id);
        }

        [Fact(DisplayName = "CreateUser with invalid request returns 400 BadRequest")]
        public async Task CreateUser_InvalidRequest_ReturnsBadRequest()
        {
            // Arrange
            var request = UsersControllerTestData.GenerateInvalidCreateUserRequest();

            // Act
            var actionResult = await _controller.CreateUser(request, CancellationToken.None);

            // Assert
            var result = actionResult as BadRequestObjectResult;
            result.Should().NotBeNull();
            result!.StatusCode.Should().Be(400);

            var errors = result.Value as IEnumerable<object>;
            errors.Should().NotBeNull(); // The validation errors
        }

        #endregion

        #region GetUser

        [Fact(DisplayName = "GetUser with valid ID returns 200 OK")]
        public async Task GetUser_ValidId_ReturnsOk()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var getUserCommand = new GetUserCommand(userId);
            var getUserResult = new GetUserResult
            {
                Id = userId,
                Name = "john_doe"
            };
            var responseDto = new GetUserResponse
            {
                Id = userId,
                Name = "john_doe"
            };

            _mediator.Send(getUserCommand, Arg.Any<CancellationToken>()).Returns(getUserResult);

            // Act
            var actionResult = await _controller.GetUser(userId, CancellationToken.None);

            // Assert
            var okResult = actionResult as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult!.StatusCode.Should().Be(200);

            var apiResponse = okResult.Value as ApiResponseWithData<GetUserResult>;
            apiResponse.Should().NotBeNull();
            apiResponse!.Data!.Id.Should().Be(userId);
            apiResponse.Data.Name.Should().Be("john_doe");
        }

        [Fact(DisplayName = "GetUser with invalid ID returns 400 BadRequest")]
        public async Task GetUser_InvalidId_ReturnsBadRequest()
        {
            // ID = Guid.Empty => fails request validation
            var actionResult = await _controller.GetUser(Guid.Empty, CancellationToken.None);

            var result = actionResult as BadRequestObjectResult;
            result.Should().NotBeNull();
            result!.StatusCode.Should().Be(400);
        }

        [Fact(DisplayName = "GetUser not found returns KeyNotFound => 404 Not Found (with global filter)")]
        public async Task GetUser_NotFound_ThrowsKeyNotFoundException()
        {
            var userId = Guid.NewGuid();
            var getUserCommand = new GetUserCommand(userId);

            _mediator
                .Send(getUserCommand, Arg.Any<CancellationToken>())
                .Returns<Task<GetUserResult>>(x => throw new KeyNotFoundException($"User with ID {userId} not found"));

            // Act
            Func<Task> act = () => _controller.GetUser(userId, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<KeyNotFoundException>()
                .WithMessage($"User with ID {userId} not found");
        }

        #endregion

        #region DeleteUser

        [Fact(DisplayName = "DeleteUser with valid ID returns 200 OK")]
        public async Task DeleteUser_ValidId_ReturnsOk()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var deleteUserCommand = new DeleteUserCommand(userId);

            // Act
            var actionResult = await _controller.DeleteUser(userId, CancellationToken.None);

            // Assert
            var okResult = actionResult as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult!.StatusCode.Should().Be(200);

            var apiResponse = okResult.Value as ApiResponse;
            apiResponse.Should().NotBeNull();
            apiResponse!.Success.Should().BeTrue();
        }

        [Fact(DisplayName = "DeleteUser with invalid ID returns 400 BadRequest")]
        public async Task DeleteUser_InvalidId_ReturnsBadRequest()
        {
            // ID = Guid.Empty => fails request validation
            var actionResult = await _controller.DeleteUser(Guid.Empty, CancellationToken.None);

            var badRequestResult = actionResult as BadRequestObjectResult;
            badRequestResult.Should().NotBeNull();
            badRequestResult!.StatusCode.Should().Be(400);
        }

        #endregion
    }
}
