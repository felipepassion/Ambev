using Ambev.DeveloperEvaluation.Integration.Factories;
using Ambev.DeveloperEvaluation.WebApi.Common;
using Ambev.DeveloperEvaluation.WebApi.Features.Branches.CreateBranch;
using Ambev.DeveloperEvaluation.WebApi.Features.Branches.GetBranch;
using FluentAssertions;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace Ambev.DeveloperEvaluation.Integration.Routes;

/// <summary>
/// Integration tests for Branches endpoints
/// </summary>
public class BranchesIntegrationTests : IClassFixture<IntegrationTestFactory>
{
    private readonly HttpClient _client;

    public BranchesIntegrationTests(IntegrationTestFactory factory)
    {
        // Cria client com a aplicação “subida” e DB InMemory
        _client = factory.CreateClient();
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

        // Lê o JSON retornado
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

        // Retorna lista de errors ou algo do tipo
        var errors = await response.Content.ReadFromJsonAsync<List<object>>();
        errors.Should().NotBeNull();
        errors!.Count.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task GetBranch_ShouldReturn200_WhenExists()
    {
        // Primeiro criar um branch
        var createReq = new CreateBranchRequest { Name = "GetBranchTest" };
        var createResp = await _client.PostAsJsonAsync("/api/branches", createReq);
        createResp.StatusCode.Should().Be(HttpStatusCode.Created);

        var createdApiResp = await createResp.Content.ReadFromJsonAsync<ApiResponseWithData<CreateBranchResponse>>();
        var createdId = createdApiResp!.Data!.Id;

        // Act - agora faz GET
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
        // Guid.Empty => triggers validation
        var resp = await _client.GetAsync($"/api/branches/{Guid.Empty}");
        resp.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task DeleteBranch_ShouldReturn200_AndThen404WhenGet()
    {
        // 1) Cria branch
        var createReq = new CreateBranchRequest { Name = "BranchToDelete" };
        var createResp = await _client.PostAsJsonAsync("/api/branches", createReq);
        createResp.StatusCode.Should().Be(HttpStatusCode.Created);
        var createdData = await createResp.Content.ReadFromJsonAsync<ApiResponseWithData<CreateBranchResponse>>();
        var branchId = createdData!.Data!.Id;

       
        var delResp = await _client.DeleteAsync($"/api/branches/{branchId}");
        delResp.StatusCode.Should().Be(HttpStatusCode.OK);

        // 3) Tenta get => 404 ou  KeyNotFound => depende do seu global filter
        var getResp = await _client.GetAsync($"/api/branches/{branchId}");
        // Se vc converte KeyNotFoundException => 404, então:
        getResp.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
