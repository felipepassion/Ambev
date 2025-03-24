using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Events.Products;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using FluentValidation;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Products.CreateProduct;

/// <summary>
/// Handler for processing CreateProductCommand requests.
/// Validates the command, creates a new product, persists it, and publishes a ProductCreatedEvent.
/// </summary>
public class CreateProductHandler : IRequestHandler<CreateProductCommand, CreateProductResult>
{
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;
    private readonly IMediator _mediator;

    public CreateProductHandler(
        IMapper mapper,
        IMediator mediator,
        IProductRepository productRepository)
    {
        _mapper = mapper;
        _mediator = mediator;
        _productRepository = productRepository;
    }

    public async Task<CreateProductResult> Handle(CreateProductCommand command, CancellationToken cancellationToken)
    {
        var validator = new CreateProductCommandValidator();
        var validationResult = await validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var product = _mapper.Map<Product>(command);

        var createdProduct = await _productRepository.CreateAsync(product, cancellationToken);

        var productCreatedEvent = new ProductCreatedEvent(
            createdProduct.Id,
            createdProduct.Name,
            createdProduct.Description,
            createdProduct.UnitPrice
        );
        await _mediator.Publish(productCreatedEvent, cancellationToken);

        return _mapper.Map<CreateProductResult>(createdProduct);
    }
}
