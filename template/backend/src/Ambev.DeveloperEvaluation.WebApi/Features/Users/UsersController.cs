using Ambev.DeveloperEvaluation.Application.Users.CreateUser;
using Ambev.DeveloperEvaluation.Application.Users.DeleteUser;
using Ambev.DeveloperEvaluation.Application.Users.GetUser;
using Ambev.DeveloperEvaluation.Application.Users.GetUsers;
using Ambev.DeveloperEvaluation.WebApi.Common;
using Ambev.DeveloperEvaluation.WebApi.Features.Users.CreateUser;
using Ambev.DeveloperEvaluation.WebApi.Features.Users.DeleteUser;
using Ambev.DeveloperEvaluation.WebApi.Features.Users.GetUser;
using Ambev.DeveloperEvaluation.WebApi.Features.Users.ListUsers;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;
using System.Net;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Users;

/// <summary>
/// Controller for managing user operations
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class UsersController : BaseController
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;

    /// <summary>
    /// Initializes a new instance of UsersController
    /// </summary>
    /// <param name="mediator">The mediator instance</param>
    /// <param name="mapper">The AutoMapper instance</param>
    public UsersController(IMediator mediator, IMapper mapper)
    {
        _mediator = mediator;
        _mapper = mapper;
    }

    /// <summary>
    /// Create User
    /// </summary>
    /// <param name="request">Contains the details required to create a new User.</param>
    /// <param name="cancellationToken">Allows the operation to be canceled if needed.</param>
    /// <returns>Returns a response indicating the success or failure of the User creation.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    [SwaggerResponseExample((int)HttpStatusCode.Created, typeof(Docs.Samples.UserResponseSamples.CreateUserResponseSample))]
    [SwaggerResponseExample((int)HttpStatusCode.BadRequest, typeof(Docs.Samples.UserErrorSamples.BadRequestErrorSample))]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request, CancellationToken cancellationToken)
    {
        var validator = new CreateUserRequestValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);
        var command = _mapper.Map<CreateUserCommand>(request);
        var response = await _mediator.Send(command, cancellationToken);
        return Created(string.Empty, new ApiResponseWithData<CreateUserResponse>
        {
            Success = true,
            Message = "User created successfully",
            Data = _mapper.Map<CreateUserResponse>(response)
        });
    }

    /// <summary>
    /// Delete User
    /// </summary>
    /// <param name="id">The unique identifier for the User to be deleted.</param>
    /// <param name="cancellationToken">Used to signal cancellation of the delete operation if needed.</param>
    /// <returns>Returns a success response if the User is deleted successfully or a bad request if validation fails.</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(object))]
    [SwaggerResponse((int)HttpStatusCode.BadRequest, "BadRequest", typeof(object))]
    [SwaggerResponseExample((int)HttpStatusCode.BadRequest, typeof(Docs.Samples.UserErrorSamples.BadRequestErrorSample))]
    [SwaggerResponseExample((int)HttpStatusCode.OK, typeof(Docs.Samples.UserResponseSamples.DeleteUserResponseSample))]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    [SwaggerResponseExample((int)HttpStatusCode.NotFound, typeof(Docs.Samples.UserErrorSamples.NotFoundErrorSample))]
    public async Task<IActionResult> DeleteUser([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var request = new DeleteUserRequest { Id = id };
        var validator = new DeleteUserRequestValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);
        var command = _mapper.Map<DeleteUserCommand>(request.Id);
        await _mediator.Send(command, cancellationToken);
        return Ok(new ApiResponse
        {
            Success = true,
            Message = "User deleted successfully"
        });
    }

    /// <summary>
    /// Get User By Id
    /// </summary>
    /// <param name="id">The unique identifier for the User to be deleted.</param>
    /// <param name="cancellationToken">Used to signal cancellation of the delete operation if needed.</param>
    /// <returns>Returns a User</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponseWithData<GetUserResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    [SwaggerResponseExample((int)HttpStatusCode.OK, typeof(Docs.Samples.UserResponseSamples.GetUserResponseSample))]
    [SwaggerResponseExample((int)HttpStatusCode.BadRequest, typeof(Docs.Samples.UserErrorSamples.BadRequestErrorSample))]
    [SwaggerResponseExample((int)HttpStatusCode.NotFound, typeof(Docs.Samples.UserErrorSamples.NotFoundErrorSample))]
    public async Task<IActionResult> GetUser([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var request = new GetUserRequest { Id = id };
        var validator = new GetUserRequestValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);
        var command = _mapper.Map<GetUserCommand>(request.Id);
        var response = await _mediator.Send(command, cancellationToken);
        var mappedResponse = _mapper.Map<GetUserResponse>(response);
        return Ok(mappedResponse, "User retrieved successfully");
    }

    /// <summary>
    /// Get Users List (Paginated)
    /// </summary>
    /// <param name="query">Contains the criteria for filtering and paginating the list of users.</param>
    /// <param name="cancellationToken">Allows the operation to be canceled if needed, providing control over long-running tasks.</param>
    /// <returns>Returns a paginated list of users wrapped in an IActionResult.</returns>
    [HttpGet("list")]
    [SwaggerResponseExample((int)HttpStatusCode.OK, typeof(Docs.Samples.UserResponseSamples.GetUsersListResponseSample))]
    [ProducesResponseType(typeof(ApiResponseWithData<GetUserResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(IActionResult), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUsers([FromQuery] GetUsersQuery query, CancellationToken cancellationToken)
    {
        var pagedList = await _mediator.Send(new GetUsersCommand(query.PageNumber, query.PageSize), cancellationToken);
        var responseList = _mapper.Map<List<GetUserResponse>>(pagedList);
        return OkPaginated(new PaginatedList<GetUserResponse>(responseList, 20, query.PageNumber, query.PageSize));
    }
}
