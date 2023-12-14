using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BadNews.Controllers
{
    public class ErrorsController : Controller
    {
        private ILogger<ErrorsController> Logger { get; init; }
        public ErrorsController(ILogger<ErrorsController> logger)
        {
            Logger = logger;
        }
        public IActionResult Exception()
        {
            return View(null, HttpContext.TraceIdentifier);
        }
        public IActionResult StatusCode(int? code)
        {
            Logger.LogWarning("status-code {code} at {time}", code, DateTime.Now);
            return View(code);
        }
    }
}