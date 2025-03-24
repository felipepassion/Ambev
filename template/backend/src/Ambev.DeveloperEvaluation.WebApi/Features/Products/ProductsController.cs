using Ambev.DeveloperEvaluation.Application.Products.CreateProduct;
using Ambev.DeveloperEvaluation.Application.Products.DeleteProduct;
using Ambev.DeveloperEvaluation.Application.Products.GetProduct;
using Ambev.DeveloperEvaluation.WebApi.Common;
using Ambev.DeveloperEvaluation.WebApi.Features.Products.CreateProduct;
using Ambev.DeveloperEvaluation.WebApi.Features.Products.DeleteProduct;
using Ambev.DeveloperEvaluation.WebApi.Features.Products.GetProduct;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;
using System.Net;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Products;

/// <summary>
/// Handles Product-related operations such as creating, deleting, and retrieving Products. Utilizes mediator and
/// mapper for processing requests.
/// </summary>
[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ProductsController : BaseController
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;

    /// <summary>
    /// Initializes a new instance of the ProductsController class.
    /// </summary>
    /// <param name="mediator">Facilitates communication between different parts of the application.</param>
    /// <param name="mapper">Transforms data between different object types for easier handling.</param>
    public ProductsController(IMediator mediator, IMapper mapper)
    {
        _mediator = mediator;
        _mapper = mapper;
    }

    /// <summary>
    /// Create Product
    /// </summary>
    /// <param name="request">Contains the details required to create a new Product.</param>
    /// <param name="cancellationToken">Allows the operation to be canceled if needed.</param>
    /// <returns>Returns a response indicating the success or failure of the Product creation.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    [SwaggerResponseExample((int)HttpStatusCode.Created, typeof(Docs.Samples.ProductResponseSamples.CreateProductResponseSample))]
    [SwaggerResponseExample((int)HttpStatusCode.BadRequest, typeof(Docs.Samples.ProductErrorSamples.BadRequestErrorSample))]
    public async Task<IActionResult> CreateProduct([FromBody] CreateProductRequest request, CancellationToken cancellationToken)
    {
        var validator = new CreateProductRequestValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);
        var command = _mapper.Map<CreateProductCommand>(request);
        var response = await _mediator.Send(command, cancellationToken);
        return Created(string.Empty, new ApiResponseWithData<CreateProductResponse>
        {
            Success = true,
            Message = "Product created successfully",
            Data = _mapper.Map<CreateProductResponse>(response)
        });
    }

    /// <summary>
    /// Delete Product
    /// </summary>
    /// <param name="id">The unique identifier for the Product to be deleted.</param>
    /// <param name="cancellationToken">Used to signal cancellation of the delete operation if needed.</param>
    /// <returns>Returns a success response if the Product is deleted successfully or a bad request if validation fails.</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(object))]
    [SwaggerResponse((int)HttpStatusCode.BadRequest, "BadRequest", typeof(object))]
    [SwaggerResponseExample((int)HttpStatusCode.BadRequest, typeof(Docs.Samples.ProductErrorSamples.BadRequestErrorSample))]
    [SwaggerResponseExample((int)HttpStatusCode.OK, typeof(Docs.Samples.ProductResponseSamples.DeleteProductResponseSample))]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [SwaggerResponseExample((int)HttpStatusCode.NotFound, typeof(Docs.Samples.ProductErrorSamples.NotFoundErrorSample))]
    public async Task<IActionResult> DeleteProduct([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var request = new DeleteProductRequest { Id = id };
        var validator = new DeleteProductRequestValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);
        var command = _mapper.Map<DeleteProductCommand>(request.Id);
        await _mediator.Send(command, cancellationToken);
        return Ok(new ApiResponse
        {
            Success = true,
            Message = "Product deleted successfully"
        });
    }

    /// <summary>
    /// Get Product By Id
    /// </summary>
    /// <param name="id">The unique identifier for the Product to be deleted.</param>
    /// <param name="cancellationToken">Used to signal cancellation of the delete operation if needed.</param>
    /// <returns>Returns a Product</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponseWithData<GetProductResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    [SwaggerResponseExample((int)HttpStatusCode.OK, typeof(Docs.Samples.ProductResponseSamples.GetProductResponseSample))]
    [SwaggerResponseExample((int)HttpStatusCode.BadRequest, typeof(Docs.Samples.ProductErrorSamples.BadRequestErrorSample))]
    [SwaggerResponseExample((int)HttpStatusCode.NotFound, typeof(Docs.Samples.ProductErrorSamples.NotFoundErrorSample))]
    public async Task<IActionResult> GetProduct([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var request = new GetProductRequest { Id = id };
        var validator = new GetProductRequestValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);
        var command = _mapper.Map<GetProductCommand>(request.Id);
        var response = await _mediator.Send(command, cancellationToken);
        var mappedResponse = _mapper.Map<GetProductResponse>(response);
        return Ok(mappedResponse, "Product retrieved successfully");
    }
}