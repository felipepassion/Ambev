using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace Ambev.DeveloperEvaluation.Integration.Filters
{
public class FakeUserFilter : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.Name, "Fake")
        };
        var identity = new ClaimsIdentity(claims, "Fake");
        context.HttpContext.User = new ClaimsPrincipal(identity);
        await next();
    }
}
}