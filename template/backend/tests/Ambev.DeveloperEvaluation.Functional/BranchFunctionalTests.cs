using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Xunit;

namespace Ambev.DeveloperEvaluation.FunctionalTests
{
    public class UserCreationAndBranchTests : IClassFixture<FunctionalTestFactory>, IAsyncLifetime
    {
        private readonly HttpClient _client;
        private readonly string _userEmail = "user@example.com";
        private readonly string _userPassword = "P@ssw0rd123";

        public UserCreationAndBranchTests(FunctionalTestFactory factory)
        {
            _client = factory.CreateClient();
        }

        public async Task InitializeAsync()
        {
            var userRequest = new
            {
                Email = _userEmail,
                Password = _userPassword,
                Username = "TestUser",
                Phone = "21969791929",
                Role = 1,
                Status = 1
            };

            var json = JsonSerializer.Serialize(userRequest);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("/api/users", content);
            var badRequestMessage = response.StatusCode == System.Net.HttpStatusCode.BadRequest
                ? await response.Content.ReadAsStringAsync()
                : null;
            response.EnsureSuccessStatusCode();
        }

        public Task DisposeAsync() => Task.CompletedTask;

        private async Task<string> AuthenticateAsync()
        {
            var authRequest = new
            {
                Email = _userEmail,
                Password = _userPassword
            };

            var json = JsonSerializer.Serialize(authRequest);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("/api/auth", content);
            response.EnsureSuccessStatusCode();
            var responseJson = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(responseJson);
            var token = doc.RootElement.GetProperty("data").GetProperty("token").GetString();
            return token!;
        }

        [Fact]
        public async Task Get_Branch_With_CreatedUser_ReturnsOk()
        {
            var token = await AuthenticateAsync();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _client.GetAsync($"/api/branches/{FunctionalTestFactory.Branch_ID}");
            response.EnsureSuccessStatusCode();
        }
    }
}
