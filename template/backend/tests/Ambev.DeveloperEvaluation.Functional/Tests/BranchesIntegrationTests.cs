using Ambev.DeveloperEvaluation.Functional.Extensions;
using Ambev.DeveloperEvaluation.Functional.Factories;
using Ambev.DeveloperEvaluation.WebApi.Common;
using Ambev.DeveloperEvaluation.WebApi.Features.Branches.CreateBranch;
using Ambev.DeveloperEvaluation.WebApi.Features.Branches.GetBranch;
using FluentAssertions;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace Ambev.DeveloperEvaluation.Functional.Tests;

/// <summary>
/// Integration tests for Branches endpoints
/// </summary>
public class BranchesIntegrationTests : IClassFixture<BranchesIntegrationTestFactory>
{
    private readonly HttpClient _client;

    public BranchesIntegrationTests(BranchesIntegrationTestFactory factory)
    {
        _client = factory.CreateClient();
        _client.SetAuthenticationAsync().Wait();
    }

    [Fact]
    public async Task CreateBranch_ShouldReturn201AndPersist()
    {
        // Arrange
        var request = new CreateBranchRequest
        {
            Name = "Test Branch Integration"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/branches", request);

        // Assert - status
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseWithData<CreateBranchResponse>>();
        apiResponse.Should().NotBeNull();
        apiResponse!.Success.Should().BeTrue();
        apiResponse.Data!.Id.Should().NotBe(Guid.Empty);
        apiResponse.Data.Name.Should().Be("Test Branch Integration");
    }

    [Fact]
    public async Task CreateBranch_WithInvalidData_ShouldReturn400()
    {
        // Arrange
        var request = new CreateBranchRequest
        {
            Name = "" // invalid
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/branches", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var errors = await response.Content.ReadFromJsonAsync<List<object>>();
        errors.Should().NotBeNull();
        errors!.Count.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task GetBranch_ShouldReturn200_WhenExists()
    {
        // Arrange
        var createReq = new CreateBranchRequest { Name = "GetBranchTest" };
        var createResp = await _client.PostAsJsonAsync("/api/branches", createReq);
        createResp.StatusCode.Should().Be(HttpStatusCode.Created);

        var createdApiResp = await createResp.Content.ReadFromJsonAsync<ApiResponseWithData<CreateBranchResponse>>();
        var createdId = createdApiResp!.Data!.Id;

        // Act
        var getResp = await _client.GetAsync($"/api/branches/{createdId}");
        getResp.StatusCode.Should().Be(HttpStatusCode.OK);

        var getApiResp = await getResp.Content.ReadFromJsonAsync<ApiResponseWithData<GetBranchResponse>>();
        getApiResp.Should().NotBeNull();
        getApiResp!.Data!.Id.Should().Be(createdId);
        getApiResp.Data.Name.Should().Be("GetBranchTest");
        getApiResp.Data.IsActive.Should().BeTrue(); // normal
    }

    [Fact]
    public async Task GetBranch_WithInvalidId_ShouldReturn400()
    {
        var resp = await _client.GetAsync($"/api/branches/{Guid.Empty}");
        resp.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task DeleteBranch_ShouldReturn200_AndThen404WhenGet()
    {
        var createReq = new CreateBranchRequest { Name = "BranchToDelete" };
        var createResp = await _client.PostAsJsonAsync("/api/branches", createReq);
        createResp.StatusCode.Should().Be(HttpStatusCode.Created);
        var createdData = await createResp.Content.ReadFromJsonAsync<ApiResponseWithData<CreateBranchResponse>>();
        var branchId = createdData!.Data!.Id;
       
        var delResp = await _client.DeleteAsync($"/api/branches/{branchId}");
        delResp.StatusCode.Should().Be(HttpStatusCode.OK);

        var getResp = await _client.GetAsync($"/api/branches/{branchId}");
        getResp.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
