using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Ambev.DeveloperEvaluation.Integration;

public class FakeUserMiddleware
{
    private readonly RequestDelegate _next;
    public FakeUserMiddleware(RequestDelegate next) => _next = next;
    public async Task Invoke(HttpContext context)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, "1"),
            new Claim(ClaimTypes.Name, "FakeUser")
        };
        var identity = new ClaimsIdentity(claims, "Fake");
        context.User = new ClaimsPrincipal(identity);
        await _next(context);
    }
}
