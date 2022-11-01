using Microsoft.AspNetCore.Mvc;

namespace BadNews.Controllers
{
    public class ErrorsController : Controller
    {
        private readonly ILogger<ErrorsController> logger;

        public ErrorsController(ILogger<ErrorsController> logger)
        {
            this.logger = logger;
        }

        public IActionResult StatusCode(int? code)
        {
            logger.LogWarning("status-code {code} at {time}", code, DateTime.Now);
            return View(code);
        }

        public IActionResult Exception()
        {
            View(null, HttpContext.TraceIdentifier)
            return View(code);
        }
    }
}