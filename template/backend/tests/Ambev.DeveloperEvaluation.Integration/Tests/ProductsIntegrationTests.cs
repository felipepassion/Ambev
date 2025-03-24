using Ambev.DeveloperEvaluation.Application;
using Ambev.DeveloperEvaluation.Application.Products.GetProduct;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Integration.TestsData;
using Ambev.DeveloperEvaluation.ORM;
using Ambev.DeveloperEvaluation.ORM.Repositories;
using Ambev.DeveloperEvaluation.WebApi;
using Ambev.DeveloperEvaluation.WebApi.Common;
using Ambev.DeveloperEvaluation.WebApi.Features.Products;
using Ambev.DeveloperEvaluation.WebApi.Features.Products.CreateProduct;
using Ambev.DeveloperEvaluation.WebApi.Features.Products.GetProduct;
using AutoMapper;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Ambev.DeveloperEvaluation.Integration.Tests
{
    public class ProductsIntegrationTests : IAsyncLifetime
    {
        private readonly ServiceProvider _provider;
        private readonly DefaultContext _context;
        private readonly ProductsController _controller;

        public ProductsIntegrationTests()
        {
            var services = new ServiceCollection();

            // Use an in-memory DB for testing
            services.AddDbContext<DefaultContext>(options =>
                options.UseInMemoryDatabase("TestProductsDb"));

            services.AddScoped<IProductRepository, ProductRepository>();

            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssemblies(
                    typeof(ApplicationLayer).Assembly,
                    typeof(Program).Assembly
                );
            });

            services.AddAutoMapper(cfg =>
            {
                cfg.AddProfile<Application.Products.GetProduct.GetProductProfile>();
                cfg.AddProfile<WebApi.Features.Products.CreateProduct.CreateProductProfile>();
                cfg.AddProfile<Application.Products.CreateProduct.CreateProductProfile>();
                cfg.AddProfile<WebApi.Features.Products.GetProduct.GetProductProfile>();
            });

            _provider = services.BuildServiceProvider();

            _context = _provider.GetRequiredService<DefaultContext>();

            var mediator = _provider.GetRequiredService<IMediator>();
            var mapper = _provider.GetRequiredService<IMapper>();
            _controller = new ProductsController(mediator, mapper);
        }

        public Task InitializeAsync() => Task.CompletedTask;

        public async Task DisposeAsync()
        {
            await _context.Database.EnsureDeletedAsync();
            _provider.Dispose();
        }

        #region CreateProduct

        [Fact(DisplayName = "CreateProduct returns 201 and persists data")]
        public async Task CreateProduct_ValidRequest_ReturnsCreated()
        {
            // Arrange
            var request = ProductsIntegrationTestData.GenerateValidCreateProductRequest();

            // Act
            var actionResult = await _controller.CreateProduct(request, CancellationToken.None);

            // Assert
            var createdResult = actionResult as CreatedResult;
            createdResult.Should().NotBeNull();
            createdResult!.StatusCode.Should().Be(201);

            var apiResponse = createdResult.Value as ApiResponseWithData<CreateProductResponse>;
            apiResponse.Should().NotBeNull();
            apiResponse!.Data!.Id.Should().NotBe(Guid.Empty);
            apiResponse.Data.Name.Should().Be(request.Name);

            // Verify from the database
            var productFromDb = await _context.Products.FindAsync(apiResponse.Data!.Id);
            productFromDb.Should().NotBeNull();
            productFromDb!.Name.Should().Be(request.Name);
        }

        [Fact(DisplayName = "CreateProduct with invalid request returns 400 BadRequest")]
        public async Task CreateProduct_InvalidRequest_ReturnsBadRequest()
        {
            // Arrange
            var request = ProductsIntegrationTestData.GenerateInvalidCreateProductRequest();

            // Act
            var actionResult = await _controller.CreateProduct(request, CancellationToken.None);

            // Assert
            var badRequest = actionResult as BadRequestObjectResult;
            badRequest.Should().NotBeNull();
            badRequest!.StatusCode.Should().Be(400);

            var errors = badRequest.Value as IEnumerable<object>;
            errors.Should().NotBeNull();
            errors!.Should().NotBeEmpty();
        }

        #endregion

        #region GetProduct

        [Fact(DisplayName = "GetProduct returns 200 when product exists")]
        public async Task GetProduct_ValidId_ReturnsOk()
        {
            // Arrange: create a product
            var createRequest = ProductsIntegrationTestData.GenerateValidCreateProductRequest();
            var createActionResult = await _controller.CreateProduct(createRequest, CancellationToken.None);
            var createdResult = createActionResult as CreatedResult;
            createdResult.Should().NotBeNull();

            var createdApiResponse = createdResult.Value as ApiResponseWithData<CreateProductResponse>;
            var productId = createdApiResponse!.Data!.Id;

            // Act: call GetProduct
            var getActionResult = await _controller.GetProduct(productId, CancellationToken.None);

            // Assert
            var okResult = getActionResult as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult!.StatusCode.Should().Be(200);

            var getApiResponse = okResult.Value as ApiResponseWithData<GetProductResponse>;
            getApiResponse.Should().NotBeNull();
            getApiResponse!.Data!.Id.Should().Be(productId);
            getApiResponse.Data.Name.Should().Be(createRequest.Name);
            getApiResponse.Data.IsActive.Should().BeTrue();
        }

        [Fact(DisplayName = "GetProduct with invalid ID returns 400 BadRequest")]
        public async Task GetProduct_InvalidId_ReturnsBadRequest()
        {
            // Act
            var actionResult = await _controller.GetProduct(Guid.Empty, CancellationToken.None);

            // Assert
            var badRequest = actionResult as BadRequestObjectResult;
            badRequest.Should().NotBeNull();
            badRequest!.StatusCode.Should().Be(400);
        }

        [Fact(DisplayName = "GetProduct not found throws KeyNotFoundException")]
        public async Task GetProduct_NotFound_ThrowsKeyNotFoundException()
        {
            // Arrange: create & delete the product to simulate not found
            var createRequest = ProductsIntegrationTestData.GenerateValidCreateProductRequest();
            var createActionResult = await _controller.CreateProduct(createRequest, CancellationToken.None);
            var createdResult = createActionResult as CreatedResult;
            createdResult.Should().NotBeNull();

            var createdApiResponse = createdResult.Value as ApiResponseWithData<CreateProductResponse>;
            var productId = createdApiResponse!.Data!.Id;

            // Delete the product
            var deleteResult = await _controller.DeleteProduct(productId, CancellationToken.None);
            deleteResult.Should().BeOfType<OkObjectResult>();

            // Act
            Func<Task> act = () => _controller.GetProduct(productId, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<KeyNotFoundException>();
        }

        #endregion

        #region DeleteProduct

        [Fact(DisplayName = "DeleteProduct returns 200 and subsequent Get throws KeyNotFoundException")]
        public async Task DeleteProduct_ReturnsOkAndThenThrowsNotFound()
        {
            // Arrange: create a product
            var createRequest = ProductsIntegrationTestData.GenerateValidCreateProductRequest();
            var createActionResult = await _controller.CreateProduct(createRequest, CancellationToken.None);
            var createdResult = createActionResult as CreatedResult;
            createdResult.Should().NotBeNull();

            var createdApiResponse = createdResult.Value as ApiResponseWithData<CreateProductResponse>;
            var productId = createdApiResponse!.Data!.Id;

            var deleteActionResult = await _controller.DeleteProduct(productId, CancellationToken.None);
            var okDeleteResult = deleteActionResult as OkObjectResult;
            okDeleteResult.Should().NotBeNull();
            okDeleteResult!.StatusCode.Should().Be(200);

            Func<Task> act = () => _controller.GetProduct(productId, CancellationToken.None);
            await act.Should().ThrowAsync<KeyNotFoundException>();
        }

        [Fact(DisplayName = "DeleteProduct with invalid ID returns 400 BadRequest")]
        public async Task DeleteProduct_InvalidId_ReturnsBadRequest()
        {
            // Act
            var actionResult = await _controller.DeleteProduct(Guid.Empty, CancellationToken.None);

            // Assert
            var badRequest = actionResult as BadRequestObjectResult;
            badRequest.Should().NotBeNull();
            badRequest!.StatusCode.Should().Be(400);
        }

        #endregion
    }
}
