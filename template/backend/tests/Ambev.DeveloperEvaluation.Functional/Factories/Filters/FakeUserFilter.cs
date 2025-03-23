using Ambev.DeveloperEvaluation.Functional.Extensions;
using Ambev.DeveloperEvaluation.Functional.Tests;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

public class FakeUserFilter : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, SalesIntegrationTests.User_ID.ToString()),
            new Claim(ClaimTypes.Name, HttpClientExtensions.USERNAME)
        };
        var identity = new ClaimsIdentity(claims, "Fake");
        context.HttpContext.User = new ClaimsPrincipal(identity);
        await next();
    }
}
