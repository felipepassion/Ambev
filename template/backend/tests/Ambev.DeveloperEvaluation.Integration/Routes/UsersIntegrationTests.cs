using Ambev.DeveloperEvaluation.Integration.Factories;
using Ambev.DeveloperEvaluation.WebApi.Common;
using Ambev.DeveloperEvaluation.WebApi.Features.Users.CreateUser;
using Ambev.DeveloperEvaluation.WebApi.Features.Users.GetUser;
using FluentAssertions;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace Ambev.DeveloperEvaluation.Integration.Routes;

/// <summary>
/// Integration tests for Users endpoints.
/// </summary>
public class UsersIntegrationTests : IClassFixture<IntegrationTestFactory>
{
    private readonly HttpClient _client;

    public UsersIntegrationTests(IntegrationTestFactory factory)
    {
        // Cria um HttpClient que aponta para a aplicação "subida" via InMemory DB
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task CreateUser_ShouldReturn201AndPersist()
    {
        // Arrange
        var createReq = new CreateUserRequest
        {
            Username = "john_doe_integration",
            Password = "Test@1234",
            Email = "john_integration@example.com",
            Phone = "+5511999999999",
            Status = Domain.Enums.UserStatus.Active,
            Role = Domain.Enums.UserRole.Customer
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/users", createReq);

        // Assert - status
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        // Lê o JSON retornado
        var apiResponse =
            await response.Content.ReadFromJsonAsync<ApiResponseWithData<CreateUserResponse>>();
        apiResponse.Should().NotBeNull();
        apiResponse!.Success.Should().BeTrue();
        apiResponse.Data!.Id.Should().NotBe(Guid.Empty);
        // Se quiser checar campo por campo:
        //   apiResponse.Data.Username, etc.
    }

    [Fact]
    public async Task CreateUser_WithInvalidData_ShouldReturn400()
    {
        // Arrange
        var invalidReq = new CreateUserRequest
        {
            Username = "",     // required => invalid
            Password = "abc",  // vai falhar no password validator (se houver)
            Email = "invalid_email_format",
            Phone = "123",     // deve falhar na regex
            Status = Domain.Enums.UserStatus.Unknown,
            Role = Domain.Enums.UserRole.None
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/users", invalidReq);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var errors = await response.Content.ReadFromJsonAsync<List<object>>();
        errors.Should().NotBeNull();
        errors!.Count.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task GetUser_ShouldReturn200_WhenExists()
    {
        
        var createReq = new CreateUserRequest
        {
            Username = "getuser_integration",
            Password = "Abc@12345",
            Email = "get_user@example.com",
            Phone = "+5511999999998",
            Status = Domain.Enums.UserStatus.Active,
            Role = Domain.Enums.UserRole.Manager
        };
        var createResp = await _client.PostAsJsonAsync("/api/users", createReq);
        createResp.StatusCode.Should().Be(HttpStatusCode.Created);

        var createdApiResp =
            await createResp.Content.ReadFromJsonAsync<ApiResponseWithData<CreateUserResponse>>();
        var userId = createdApiResp!.Data!.Id;

        // 2) Faz GET
        var getResp = await _client.GetAsync($"/api/users/{userId}");
        getResp.StatusCode.Should().Be(HttpStatusCode.OK);

        var getApiResp =
            await getResp.Content.ReadFromJsonAsync<ApiResponseWithData<GetUserResponse>>();
        getApiResp.Should().NotBeNull();
        getApiResp!.Success.Should().BeTrue();
        getApiResp.Data!.Id.Should().Be(userId);
        getApiResp.Data.Email.Should().Be("get_user@example.com");
        // etc...
    }

    [Fact]
    public async Task GetUser_WithInvalidId_ShouldReturn400()
    {
        // Guid.Empty => falha na request
        var resp = await _client.GetAsync($"/api/users/{Guid.Empty}");
        resp.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task DeleteUser_ShouldReturn200_AndThen404WhenGet()
    {
        
        var createReq = new CreateUserRequest
        {
            Username = "user_todelete",
            Password = "Abc@2023",
            Email = "delete_user@example.com",
            Phone = "+5511888888888",
            Status = Domain.Enums.UserStatus.Active,
            Role = Domain.Enums.UserRole.Customer
        };
        var createResp = await _client.PostAsJsonAsync("/api/users", createReq);
        createResp.StatusCode.Should().Be(HttpStatusCode.Created);

        var createdData =
            await createResp.Content.ReadFromJsonAsync<ApiResponseWithData<CreateUserResponse>>();
        var userId = createdData!.Data!.Id;

       
        var delResp = await _client.DeleteAsync($"/api/users/{userId}");
        delResp.StatusCode.Should().Be(HttpStatusCode.OK);

        
        var getResp = await _client.GetAsync($"/api/users/{userId}");
        getResp.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
