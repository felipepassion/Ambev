using Ambev.DeveloperEvaluation.Application;
using Ambev.DeveloperEvaluation.Application.Sales.GetSale;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Integration.Filters;
using Ambev.DeveloperEvaluation.ORM;
using Ambev.DeveloperEvaluation.ORM.Repositories;
using Ambev.DeveloperEvaluation.Unit.Presentation.TestData;
using Ambev.DeveloperEvaluation.WebApi;
using Ambev.DeveloperEvaluation.WebApi.Common;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.GetSale;
using AutoMapper;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using System.Security.Claims;
using Xunit;

namespace Ambev.DeveloperEvaluation.Tests.Integration
{
    public class SalesIntegrationTests : IAsyncLifetime
    {
        private readonly DefaultContext _context;
        private readonly ServiceProvider _provider;
        private readonly SalesController _controller;

        private readonly IBranchRepository _mockBranchRepo;
        private readonly IProductRepository _mockProductRepo;

        private IHttpContextAccessor InitializeHttpContextAccessor()
        {
            var result = Substitute.For<IHttpContextAccessor>();
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "11111111-1111-1111-1111-111111111111")
            };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var principal = new ClaimsPrincipal(identity);
            var httpContext = new DefaultHttpContext { User = principal };
            result.HttpContext.Returns(httpContext);
            return result;
        }

        public SalesIntegrationTests()
        {
            var services = new ServiceCollection();

            services.AddDbContext<DefaultContext>(options =>
                options.UseInMemoryDatabase("TestSalesDb"));

            _mockBranchRepo = Substitute.For<IBranchRepository>();
            _mockProductRepo = Substitute.For<IProductRepository>();

            services.AddSingleton<ISaleRepository>(sp =>
            {
                return new SaleRepository(sp.GetRequiredService<DefaultContext>());
            });
            services.AddSingleton(_mockBranchRepo);
            services.AddSingleton(_mockProductRepo);

            services.AddHttpClient();
            services.AddScoped(x => InitializeHttpContextAccessor());
            services.AddControllers(options => { options.Filters.Add<FakeUserFilter>(); });

            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssemblies(
                    typeof(ApplicationLayer).Assembly,
                    typeof(Program).Assembly
                );
            });

            services.AddAutoMapper(cfg =>
            {
                cfg.AddProfile<Application.Sales.CreateSale.CreateSaleProfile>();
                cfg.AddProfile<Application.Sales.GetSale.GetSaleProfile>();
                cfg.AddProfile<Application.SaleItems.GetSaleItem.GetSaleItemProfile>();
                cfg.AddProfile<WebApi.Features.Sales.CreateSale.CreateSaleProfile>();
                cfg.AddProfile<WebApi.Features.Sales.GetSale.GetSaleProfile>();
            });

            _provider = services.BuildServiceProvider();
            _context = _provider.GetRequiredService<DefaultContext>();

            var mediator = _provider.GetRequiredService<IMediator>();
            var mapper = _provider.GetRequiredService<IMapper>();
            _controller = new SalesController(mediator, mapper);
        }

        public Task InitializeAsync() => Task.CompletedTask;

        public async Task DisposeAsync()
        {
            await _context.Database.EnsureDeletedAsync();
            _provider.Dispose();
        }

        [Fact(DisplayName = "CreateSale returns 201 and persists data")]
        public async Task CreateSale_ValidRequest_ReturnsCreated()
        {
            // Arrange
            var request = SalesIntegrationTestData.GenerateValidCreateSaleRequest();

            _mockBranchRepo.GetByIdAsync(request.BranchId, Arg.Any<CancellationToken>())
                .Returns(new Domain.Entities.Branch
                {
                    Id = request.BranchId,
                    Name = "Mock Branch"
                });

            foreach (var item in request.Items)
            {
                _mockProductRepo.GetByIdAsync(item.ProductId, Arg.Any<CancellationToken>())
                    .Returns(new Domain.Entities.Product
                    {
                        Id = item.ProductId,
                        Name = "Mock Product",
                        UnitPrice = 10m
                    });
            }

            // Act
            var actionResult = await _controller.CreateSale(request, CancellationToken.None);

            // Assert
            var createdResult = actionResult as CreatedResult;
            createdResult.Should().NotBeNull();
            createdResult!.StatusCode.Should().Be(201);

            var apiResponse = createdResult.Value as ApiResponseWithData<CreateSaleResponse>;
            apiResponse.Should().NotBeNull();
            apiResponse!.Data!.Id.Should().NotBe(Guid.Empty);

            var saleFromDb = await _context.Sales.FindAsync(apiResponse.Data.Id);
            saleFromDb.Should().NotBeNull();
        }

        [Fact(DisplayName = "CreateSale with invalid request returns 400 BadRequest")]
        public async Task CreateSale_InvalidRequest_ReturnsBadRequest()
        {
            // Arrange
            var request = SalesIntegrationTestData.GenerateInvalidCreateSaleRequest();

            // Act
            var actionResult = await _controller.CreateSale(request, CancellationToken.None);

            // Assert
            var badRequest = actionResult as BadRequestObjectResult;
            badRequest.Should().NotBeNull();
            badRequest!.StatusCode.Should().Be(400);

            var errors = badRequest.Value as IEnumerable<object>;
            errors.Should().NotBeNull();
            errors!.Should().NotBeEmpty();
        }

        [Fact(DisplayName = "GetSale returns 200 when sale exists")]
        public async Task GetSale_ValidId_ReturnsOk()
        {
            var createReq = SalesIntegrationTestData.GenerateValidCreateSaleRequest();

            _mockBranchRepo.GetByIdAsync(createReq.BranchId, Arg.Any<CancellationToken>())
                .Returns(new Domain.Entities.Branch
                {
                    Id = createReq.BranchId,
                    Name = "Mock Branch"
                });
            foreach (var item in createReq.Items)
            {
                _mockProductRepo.GetByIdAsync(item.ProductId, Arg.Any<CancellationToken>())
                    .Returns(new Domain.Entities.Product
                    {
                        Id = item.ProductId,
                        Name = "Mock Product",
                        UnitPrice = 10m
                    });
            }

            var createActionResult = await _controller.CreateSale(createReq, CancellationToken.None);
            var createdResult = createActionResult as CreatedResult;
            createdResult.Should().NotBeNull();
            var createdApiResponse = createdResult!.Value as ApiResponseWithData<CreateSaleResponse>;
            var saleId = createdApiResponse!.Data!.Id;

            // Act
            var getActionResult = await _controller.GetSale(saleId, CancellationToken.None);

            // Assert
            var okResult = getActionResult as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult!.StatusCode.Should().Be(200);

            var getApiResponse = okResult.Value as ApiResponseWithData<GetSaleResult>;
            getApiResponse.Should().NotBeNull();
            getApiResponse!.Data!.Id.Should().Be(saleId);
        }

        [Fact(DisplayName = "GetSale with invalid ID returns 400 BadRequest")]
        public async Task GetSale_InvalidId_ReturnsBadRequest()
        {
            // Act
            var actionResult = await _controller.GetSale(Guid.Empty, CancellationToken.None);

            // Assert
            var badRequest = actionResult as BadRequestObjectResult;
            badRequest.Should().NotBeNull();
            badRequest!.StatusCode.Should().Be(400);
        }

        [Fact(DisplayName = "GetSale not found throws KeyNotFoundException")]
        public async Task GetSale_NotFound_ThrowsKeyNotFoundException()
        {
            // Arrange
            var createReq = SalesIntegrationTestData.GenerateValidCreateSaleRequest();

            _mockBranchRepo.GetByIdAsync(createReq.BranchId, Arg.Any<CancellationToken>())
                .Returns(new Domain.Entities.Branch
                {
                    Id = createReq.BranchId,
                    Name = "Mock Branch"
                });
            foreach (var item in createReq.Items)
            {
                _mockProductRepo.GetByIdAsync(item.ProductId, Arg.Any<CancellationToken>())
                    .Returns(new Domain.Entities.Product
                    {
                        Id = item.ProductId,
                        Name = "Mock Product",
                        UnitPrice = 10m
                    });
            }

            var createActionResult = await _controller.CreateSale(createReq, CancellationToken.None);
            var createdResult = createActionResult as CreatedResult;
            createdResult.Should().NotBeNull();
            var createdApiResponse = createdResult!.Value as ApiResponseWithData<CreateSaleResponse>;
            var saleId = createdApiResponse!.Data!.Id;

            // Act: deletar e depois tentar Get
            var deleteResult = await _controller.DeleteSale(saleId, CancellationToken.None);
            deleteResult.Should().BeOfType<OkObjectResult>();

            // Assert: agora deve lançar KeyNotFoundException
            Func<Task> act = () => _controller.GetSale(saleId, CancellationToken.None);
            await act.Should().ThrowAsync<KeyNotFoundException>();
        }

        [Fact(DisplayName = "DeleteSale returns 200 and subsequent Get throws KeyNotFoundException")]
        public async Task DeleteSale_ReturnsOkAndThenThrowsNotFound()
        {
            // Arrange
            var createReq = SalesIntegrationTestData.GenerateValidCreateSaleRequest();

            _mockBranchRepo.GetByIdAsync(createReq.BranchId, Arg.Any<CancellationToken>())
                .Returns(new Domain.Entities.Branch
                {
                    Id = createReq.BranchId,
                    Name = "Mock Branch"
                });
            foreach (var item in createReq.Items)
            {
                _mockProductRepo.GetByIdAsync(item.ProductId, Arg.Any<CancellationToken>())
                    .Returns(new Domain.Entities.Product
                    {
                        Id = item.ProductId,
                        Name = "Mock Product",
                        UnitPrice = 10m
                    });
            }

            var createActionResult = await _controller.CreateSale(createReq, CancellationToken.None);
            var createdResult = createActionResult as CreatedResult;
            createdResult.Should().NotBeNull();
            var createdApiResponse = createdResult!.Value as ApiResponseWithData<CreateSaleResponse>;
            var saleId = createdApiResponse!.Data!.Id;

            // Act
            var deleteActionResult = await _controller.DeleteSale(saleId, CancellationToken.None);

            // Assert
            var okDeleteResult = deleteActionResult as OkObjectResult;
            okDeleteResult.Should().NotBeNull();
            okDeleteResult!.StatusCode.Should().Be(200);

            Func<Task> act = () => _controller.GetSale(saleId, CancellationToken.None);
            await act.Should().ThrowAsync<KeyNotFoundException>();
        }

        [Fact(DisplayName = "DeleteSale with invalid ID returns 400 BadRequest")]
        public async Task DeleteSale_InvalidId_ReturnsBadRequest()
        {
            // Act
            var actionResult = await _controller.DeleteSale(Guid.Empty, CancellationToken.None);

            // Assert
            var badRequest = actionResult as BadRequestObjectResult;
            badRequest.Should().NotBeNull();
            badRequest!.StatusCode.Should().Be(400);
        }
    }
}
