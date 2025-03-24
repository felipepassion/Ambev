using Ambev.DeveloperEvaluation.Functional.Extensions;
using Ambev.DeveloperEvaluation.Functional.Factories;
using Ambev.DeveloperEvaluation.WebApi.Common;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.GetSale;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using NSubstitute;
using System.Net;
using System.Net.Http.Json;
using System.Security.Claims;
using Xunit;

namespace Ambev.DeveloperEvaluation.Functional.Tests;

/// <summary>
/// Integration tests for the Sales endpoints.
/// </summary>
public class SalesIntegrationTests : IClassFixture<SalesIntegrationTestFactory>
{
    private readonly HttpClient _client;

    public static Guid Product1_ID => new Guid("80B1DE5F-7DB1-470A-95CF-BD81F7A3FE05");
    public static Guid Product2_ID => new Guid("5F50BD28-AF1D-453C-98AF-73200DCBEE97");
    public static Guid Branch_ID => new Guid("D6EB9EE5-8544-4A4B-8863-0DB6707AD0C5");
    public static Guid User_ID => new Guid("8E9C5B87-49E4-4FE4-96DE-7889DAB989EC");

    public SalesIntegrationTests(SalesIntegrationTestFactory factory)
    {
        _client = factory.CreateClient();
        _client.SetAuthenticationAsync().Wait();
    }

    [Fact]
    public async Task CreateSale_ShouldReturn201AndPersistData()
    {
        // Arrange
        var createSaleRequest = new CreateSaleRequest
        {
            BranchId = Branch_ID,
            Items = new List<CreateSaleItemRequest>
            {
                new CreateSaleItemRequest { ProductId = Product1_ID, Quantity = 3 },
                new CreateSaleItemRequest { ProductId = Product2_ID, Quantity = 6 }
            }
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/sales", createSaleRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var apiResponse =
            await response.Content.ReadFromJsonAsync<ApiResponseWithData<CreateSaleResponse>>();
        apiResponse.Should().NotBeNull();
        apiResponse!.Success.Should().BeTrue();
        apiResponse.Data!.Id.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public async Task CreateSale_WithInvalidData_ShouldReturn400()
    {
        // BranchId = Guid.Empty => invalid
        // Items = empty => invalid
        var invalidRequest = new CreateSaleRequest
        {
            BranchId = Guid.Empty,
            Items = new List<CreateSaleItemRequest>()
        };

        var response = await _client.PostAsJsonAsync("/api/sales", invalidRequest);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var errors = await response.Content.ReadFromJsonAsync<List<object>>();
        errors.Should().NotBeNull();
        errors!.Count.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task GetSale_ShouldReturn200_WhenSaleExists()
    {
        // 1) Create a sale
        var createReq = new CreateSaleRequest
        {
            BranchId = Branch_ID,
            Items = new List<CreateSaleItemRequest>
            {
                new CreateSaleItemRequest { ProductId = Product1_ID, Quantity = 4 }
            }
        };
        var createResp = await _client.PostAsJsonAsync("/api/sales", createReq);
        createResp.StatusCode.Should().Be(HttpStatusCode.Created);

        var createApiResp =
            await createResp.Content.ReadFromJsonAsync<ApiResponseWithData<CreateSaleResponse>>();
        var saleId = createApiResp!.Data!.Id;

        // 2) Retrieve the sale
        var getResp = await _client.GetAsync($"/api/sales/{saleId}");
        getResp.StatusCode.Should().Be(HttpStatusCode.OK);

        var getApiResp =
            await getResp.Content.ReadFromJsonAsync<ApiResponseWithData<GetSaleResponse>>();
        getApiResp.Should().NotBeNull();
        getApiResp!.Success.Should().BeTrue();
        getApiResp.Data!.Id.Should().Be(saleId);
        getApiResp.Data.Items.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetSale_WithInvalidId_ShouldReturn400()
    {
        // Guid.Empty => validation fails
        var resp = await _client.GetAsync($"/api/sales/{Guid.Empty}");
        resp.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task DeleteSale_ShouldReturn200_AndThen404WhenGettingItAgain()
    {
        // 1) Create a sale
        var createReq = new CreateSaleRequest
        {
            BranchId = Branch_ID,
            Items = new List<CreateSaleItemRequest>
            {
                new CreateSaleItemRequest { ProductId = Product1_ID, Quantity = 4 }
            }
        };
        var createResp = await _client.PostAsJsonAsync("/api/sales", createReq);
        createResp.StatusCode.Should().Be(HttpStatusCode.Created);

        var createdData =
            await createResp.Content.ReadFromJsonAsync<ApiResponseWithData<CreateSaleResponse>>();
        var saleId = createdData!.Data!.Id;

        // 2) Delete the sale
        var delResp = await _client.DeleteAsync($"/api/sales/{saleId}");
        delResp.StatusCode.Should().Be(HttpStatusCode.OK);

        // 3) Attempt to GET again => KeyNotFound => 404
        var getResp = await _client.GetAsync($"/api/sales/{saleId}");
        getResp.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
