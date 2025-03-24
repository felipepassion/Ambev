using Ambev.DeveloperEvaluation.Application.Sales.CancelSale;
using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.Application.Sales.DeleteSale;
using Ambev.DeveloperEvaluation.Application.Sales.GetSale;
using Ambev.DeveloperEvaluation.WebApi.Common;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.CancelSale;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.DeleteSale;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.GetSale;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;
using System.Net;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales;

/// <summary>
/// Controller for managing sale operations.
/// </summary>
[Authorize]
[ApiController]
[Route("api/[controller]")]
public class SalesController : BaseController
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;

    /// <summary>
    /// Initializes a new instance of the <see cref="SalesController"/> class.
    /// </summary>
    /// <param name="mediator">The MediatR instance.</param>
    /// <param name="mapper">The AutoMapper instance.</param>
    public SalesController(IMediator mediator, IMapper mapper)
    {
        _mediator = mediator;
        _mapper = mapper;
    }

    /// <summary>
    /// Create Sale
    /// </summary>
    /// <param name="request">Contains the details required to create a new Sale.</param>
    /// <param name="cancellationToken">Allows the operation to be canceled if needed.</param>
    /// <returns>Returns a response indicating the success or failure of the Sale creation.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    [SwaggerResponseExample((int)HttpStatusCode.Created, typeof(Docs.Samples.SaleResponseSamples.CreateSaleResponseSample))]
    [SwaggerResponseExample((int)HttpStatusCode.BadRequest, typeof(Docs.Samples.SaleErrorSamples.BadRequestErrorSample))]
    public async Task<IActionResult> CreateSale([FromBody] CreateSaleRequest request, CancellationToken cancellationToken)
    {
        var validator = new CreateSaleRequestValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);
        var command = _mapper.Map<CreateSaleCommand>(request);
        var response = await _mediator.Send(command, cancellationToken);
        return Created(string.Empty, new ApiResponseWithData<CreateSaleResponse>
        {
            Success = true,
            Message = "Sale created successfully",
            Data = _mapper.Map<CreateSaleResponse>(response)
        });
    }

    /// <summary>
    /// Delete Sale
    /// </summary>
    /// <param name="id">The unique identifier for the Sale to be deleted.</param>
    /// <param name="cancellationToken">Used to signal cancellation of the delete operation if needed.</param>
    /// <returns>Returns a success response if the Sale is deleted successfully or a bad request if validation fails.</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(object))]
    [SwaggerResponse((int)HttpStatusCode.BadRequest, "BadRequest", typeof(object))]
    [SwaggerResponseExample((int)HttpStatusCode.BadRequest, typeof(Docs.Samples.SaleErrorSamples.BadRequestErrorSample))]
    [SwaggerResponseExample((int)HttpStatusCode.OK, typeof(Docs.Samples.SaleResponseSamples.DeleteSaleResponseSample))]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [SwaggerResponseExample((int)HttpStatusCode.NotFound, typeof(Docs.Samples.SaleErrorSamples.NotFoundErrorSample))]
    public async Task<IActionResult> DeleteSale([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var request = new DeleteSaleRequest { Id = id };
        var validator = new DeleteSaleRequestValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);
        var command = _mapper.Map<DeleteSaleCommand>(request.Id);
        await _mediator.Send(command, cancellationToken);
        return Ok(new ApiResponse
        {
            Success = true,
            Message = "Sale deleted successfully"
        });
    }

    /// <summary>
    /// Cancel Sale
    /// </summary>
    /// <param name="id">The unique identifier for the Sale to be canceld.</param>
    /// <param name="cancellationToken">Used to signal cancellation of the cancel operation if needed.</param>
    /// <returns>Returns a success response if the Sale is canceld successfully or a bad request if validation fails.</returns>
    [HttpDelete("{id}/cancel")]
    [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(object))]
    [SwaggerResponse((int)HttpStatusCode.BadRequest, "BadRequest", typeof(object))]
    [SwaggerResponseExample((int)HttpStatusCode.BadRequest, typeof(Docs.Samples.SaleErrorSamples.BadRequestErrorSample))]
    [SwaggerResponseExample((int)HttpStatusCode.OK, typeof(Docs.Samples.SaleResponseSamples.CancelSaleResponseSample))]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [SwaggerResponseExample((int)HttpStatusCode.NotFound, typeof(Docs.Samples.SaleErrorSamples.NotFoundErrorSample))]
    public async Task<IActionResult> CancelSale([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var request = new CancelSaleRequest { Id = id };
        var validator = new CancelSaleRequestValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);
        var command = _mapper.Map<CancelSaleCommand>(request.Id);
        await _mediator.Send(command, cancellationToken);
        return Ok(new ApiResponse
        {
            Success = true,
            Message = "Sale canceld successfully"
        });
    }

    /// <summary>
    /// Get Sale By Id
    /// </summary>
    /// <param name="id">The unique identifier for the Sale to be deleted.</param>
    /// <param name="cancellationToken">Used to signal cancellation of the delete operation if needed.</param>
    /// <returns>Returns a Sale</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponseWithData<GetSaleResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    [SwaggerResponseExample((int)HttpStatusCode.OK, typeof(Docs.Samples.SaleResponseSamples.GetSaleResponseSample))]
    [SwaggerResponseExample((int)HttpStatusCode.BadRequest, typeof(Docs.Samples.SaleErrorSamples.BadRequestErrorSample))]
    [SwaggerResponseExample((int)HttpStatusCode.NotFound, typeof(Docs.Samples.SaleErrorSamples.NotFoundErrorSample))]
    public async Task<IActionResult> GetSale([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var request = new GetSaleRequest { Id = id };
        var validator = new GetSaleRequestValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);
        var command = _mapper.Map<GetSaleCommand>(request.Id);
        var response = await _mediator.Send(command, cancellationToken);
        var mappedResponse = _mapper.Map<GetSaleResponse>(response);
        return Ok(mappedResponse, "Sale retrieved successfully");
    }
}
