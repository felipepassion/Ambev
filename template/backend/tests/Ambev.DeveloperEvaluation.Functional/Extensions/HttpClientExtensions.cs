using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Ambev.DeveloperEvaluation.Functional.Extensions;

public static class HttpClientExtensions
{
    public const string USERNAME = "john_doe_integration";
    public const string USEREMAIL = "user@example.com";
    public const string USERPASSWORD = "P@ssw0rd123";

    public static async Task<string> SetAuthenticationAsync(this HttpClient _client)
    {
        _client.BaseAddress = new Uri("http://localhost");
        var authRequest = new
        {
            Email = USEREMAIL,
            Password = USERPASSWORD
        };

        var json = JsonSerializer.Serialize(authRequest);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await _client.PostAsync("/api/auth", content);
        response.EnsureSuccessStatusCode();
        var responseJson = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(responseJson);
        var token = doc.RootElement.GetProperty("data").GetProperty("token").GetString();

        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        return token!;
    }
}
