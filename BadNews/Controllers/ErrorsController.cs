using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Serilog;

namespace BadNews.Controllers
{
    public class ErrorsController : Controller
    {
        private readonly ILogger<ErrorsController> _logger;

        public ErrorsController(ILogger<ErrorsController> logger)
        {
            _logger = logger;
        }
        
        public IActionResult StatusCode(int? code)
        {
            _logger.LogWarning("status-code {code} at {time}", code, DateTime.Now);
            return View(code);
        }
        
        public IActionResult Exception()
        {
            return View(null, HttpContext.TraceIdentifier);
        }
    }
}