using Microsoft.AspNetCore.Mvc;

namespace Pokemon.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class DocsController : Controller
    {
        [HttpGet("/")]
        [HttpGet("/docs")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
