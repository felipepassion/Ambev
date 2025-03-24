using Ambev.DeveloperEvaluation.Application.Branches.CreateBranch;
using Ambev.DeveloperEvaluation.Application.Branches.DeleteBranch;
using Ambev.DeveloperEvaluation.Application.Branches.GetBranch;
using Ambev.DeveloperEvaluation.WebApi.Common;
using Ambev.DeveloperEvaluation.WebApi.Docs.Filters;
using Ambev.DeveloperEvaluation.WebApi.Features.Branches.CreateBranch;
using Ambev.DeveloperEvaluation.WebApi.Features.Branches.DeleteBranch;
using Ambev.DeveloperEvaluation.WebApi.Features.Branches.GetBranch;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;
using System.Net;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Branches
{
    /// <summary>
    /// Handles branch-related operations such as creating, deleting, and retrieving branches. Utilizes mediator and
    /// mapper for processing requests.
    /// </summary>
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class BranchesController : BaseController
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the BranchesController class.
        /// </summary>
        /// <param name="mediator">Facilitates communication between different parts of the application.</param>
        /// <param name="mapper">Transforms data between different object types for easier handling.</param>
        public BranchesController(IMediator mediator, IMapper mapper)
        {
            _mediator = mediator;
            _mapper = mapper;
        }

        /// <summary>
        /// Create Branch
        /// </summary>
        /// <param name="request">Contains the details required to create a new branch.</param>
        /// <param name="cancellationToken">Allows the operation to be canceled if needed.</param>
        /// <returns>Returns a response indicating the success or failure of the branch creation.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        [SwaggerResponseExample((int)HttpStatusCode.Created, typeof(Docs.Samples.BranchResponseSamples.CreateBranchResponseSample))]
        [SwaggerResponseExample((int)HttpStatusCode.BadRequest, typeof(Docs.Samples.BranchErrorSamples.BadRequestErrorSample))]
        public async Task<IActionResult> CreateBranch([FromBody] CreateBranchRequest request, CancellationToken cancellationToken)
        {
            var validator = new CreateBranchRequestValidator();
            var validationResult = await validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
                return BadRequest(validationResult.Errors);
            var command = _mapper.Map<CreateBranchCommand>(request);
            var response = await _mediator.Send(command, cancellationToken);
            return Created(string.Empty, new ApiResponseWithData<CreateBranchResponse>
            {
                Success = true,
                Message = "Branch created successfully",
                Data = _mapper.Map<CreateBranchResponse>(response)
            });
        }

        /// <summary>
        /// Delete Branch
        /// </summary>
        /// <param name="id">The unique identifier for the branch to be deleted.</param>
        /// <param name="cancellationToken">Used to signal cancellation of the delete operation if needed.</param>
        /// <returns>Returns a success response if the branch is deleted successfully or a bad request if validation fails.</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(object))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, "BadRequest", typeof(object))]
        [SwaggerResponseExample((int)HttpStatusCode.BadRequest, typeof(Docs.Samples.BranchErrorSamples.BadRequestErrorSample))]
        [SwaggerResponseExample((int)HttpStatusCode.OK, typeof(Docs.Samples.BranchResponseSamples.DeleteBranchResponseSample))]
        [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [SwaggerResponseExample((int)HttpStatusCode.NotFound, typeof(Docs.Samples.BranchErrorSamples.NotFoundErrorSample))]
        public async Task<IActionResult> DeleteBranch([FromRoute] Guid id, CancellationToken cancellationToken)
        {
            var request = new DeleteBranchRequest { Id = id };
            var validator = new DeleteBranchRequestValidator();
            var validationResult = await validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
                return BadRequest(validationResult.Errors);
            var command = _mapper.Map<DeleteBranchCommand>(request.Id);
            await _mediator.Send(command, cancellationToken);
            return Ok(new ApiResponse
            {
                Success = true,
                Message = "Branch deleted successfully"
            });
        }

        /// <summary>
        /// Get Branch By Id
        /// </summary>
        /// <param name="id">The unique identifier for the branch to be deleted.</param>
        /// <param name="cancellationToken">Used to signal cancellation of the delete operation if needed.</param>
        /// <returns>Returns a branch</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponseWithData<GetBranchResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
        [SwaggerResponseExample((int)HttpStatusCode.OK, typeof(Docs.Samples.BranchResponseSamples.GetBranchResponseSample))]
        [SwaggerResponseExample((int)HttpStatusCode.BadRequest, typeof(Docs.Samples.BranchErrorSamples.BadRequestErrorSample))]
        [SwaggerResponseExample((int)HttpStatusCode.NotFound, typeof(Docs.Samples.BranchErrorSamples.NotFoundErrorSample))]
        public async Task<IActionResult> GetBranch([FromRoute] Guid id, CancellationToken cancellationToken)
        {
            var request = new GetBranchRequest { Id = id };
            var validator = new GetBranchRequestValidator();
            var validationResult = await validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
                return BadRequest(validationResult.Errors);
            var command = _mapper.Map<GetBranchCommand>(request.Id);
            var response = await _mediator.Send(command, cancellationToken);
            var mappedResponse = _mapper.Map<GetBranchResponse>(response);
            return Ok(mappedResponse, "Branch retrieved successfully");
        }
    }
}