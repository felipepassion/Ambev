﻿using Ambev.DeveloperEvaluation.Functional.Extensions;
using Ambev.DeveloperEvaluation.Functional.Factories;
using Ambev.DeveloperEvaluation.WebApi.Common;
using Ambev.DeveloperEvaluation.WebApi.Features.Products.CreateProduct;
using Ambev.DeveloperEvaluation.WebApi.Features.Products.GetProduct;
using FluentAssertions;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace Ambev.DeveloperEvaluation.Functional.Tests;

/// <summary>
/// Integration tests for Products endpoints.
/// </summary>
public class ProductsIntegrationTests : IClassFixture<ProductsIntegrationTestFactory>
{
    private readonly HttpClient _client;

    public ProductsIntegrationTests(ProductsIntegrationTestFactory factory)
    {
        _client = factory.CreateClient();
        _client.SetAuthenticationAsync().Wait();
    }

    [Fact]
    public async Task CreateProduct_ShouldReturn201AndPersist()
    {
        // Arrange
        var createReq = new CreateProductRequest
        {
            Name = "Test Product Integration",
            Description = "Awesome product",
            UnitPrice = 99.99m
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/products", createReq);

        // Assert - status
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var apiResponse =
            await response.Content.ReadFromJsonAsync<ApiResponseWithData<CreateProductResponse>>();
        apiResponse.Should().NotBeNull();
        apiResponse!.Success.Should().BeTrue();
        apiResponse!.Data!.Id.Should().NotBe(Guid.Empty);
        apiResponse.Data.Name.Should().Be("Test Product Integration");
        apiResponse.Data.Description.Should().Be("Awesome product");
        apiResponse.Data.UnitPrice.Should().Be(99.99m);
    }

    [Fact]
    public async Task CreateProduct_WithInvalidData_ShouldReturn400()
    {
        // Arrange
        var createReq = new CreateProductRequest
        {
            Name = "",           // invalid (required)
            Description = null,
            UnitPrice = -1       // invalid
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/products", createReq);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var errors = await response.Content.ReadFromJsonAsync<List<object>>();
        errors.Should().NotBeNull();
        errors!.Count.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task GetProduct_ShouldReturn200_WhenExists()
    {
        var createReq = new CreateProductRequest
        {
            Name = "Test Product For Get",
            Description = "Integration Get",
            UnitPrice = 50m
        };
        var createResp = await _client.PostAsJsonAsync("/api/products", createReq);
        createResp.StatusCode.Should().Be(HttpStatusCode.Created);

        var createdApiResp =
            await createResp.Content.ReadFromJsonAsync<ApiResponseWithData<CreateProductResponse>>();
        var productId = createdApiResp!.Data!.Id;

        var getResp = await _client.GetAsync($"/api/products/{productId}");
        getResp.StatusCode.Should().Be(HttpStatusCode.OK);

        var getApiResp =
            await getResp.Content.ReadFromJsonAsync<ApiResponseWithData<GetProductResponse>>();
        getApiResp.Should().NotBeNull();
        getApiResp!.Success.Should().BeTrue();
        getApiResp.Data!.Id.Should().Be(productId);
        getApiResp.Data.Name.Should().Be("Test Product For Get");
        getApiResp.Data.UnitPrice.Should().Be(50m);
        getApiResp.Data.IsActive.Should().BeTrue();
    }

    [Fact]
    public async Task GetProduct_WithInvalidId_ShouldReturn400()
    {
        var resp = await _client.GetAsync($"/api/products/{Guid.Empty}");
        resp.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task DeleteProduct_ShouldReturn200_AndThen404WhenGet()
    {
        var createReq = new CreateProductRequest
        {
            Name = "ProductToDelete",
            Description = "Deleting soon",
            UnitPrice = 10m
        };
        var createResp = await _client.PostAsJsonAsync("/api/products", createReq);
        createResp.StatusCode.Should().Be(HttpStatusCode.Created);
        var createdData =
            await createResp.Content.ReadFromJsonAsync<ApiResponseWithData<CreateProductResponse>>();
        var productId = createdData!.Data!.Id;

        var delResp = await _client.DeleteAsync($"/api/products/{productId}");
        delResp.StatusCode.Should().Be(HttpStatusCode.OK);

        var getResp = await _client.GetAsync($"/api/products/{productId}");
        getResp.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
