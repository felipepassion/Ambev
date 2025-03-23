using Ambev.DeveloperEvaluation.Integration.Routes;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;
using System.Threading.Tasks;

public class FakeUserFilter : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, SalesIntegrationTests.User_ID.ToString()),
            new Claim(ClaimTypes.Name, "john_doe_integration")
        };
        var identity = new ClaimsIdentity(claims, "Fake");
        context.HttpContext.User = new ClaimsPrincipal(identity);
        await next();
    }
}
