using LR14.Models;
using Microsoft.AspNetCore.Mvc;
using OpenTelemetry.Trace;
using System.Diagnostics;

namespace LR14.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly Tracer _tracer;

        public HomeController(ILogger<HomeController> logger, TracerProvider tracerProvider)
        {
            _logger = logger;
            _tracer = tracerProvider.GetTracer("MyAspNetCoreTracer");
        }

        // 2
        public IActionResult Index()
        {
            _logger.Log(LogLevel.Information, "Some string!");

            return View();
        }

        // 3!-4
        //public IActionResult Index()
        //{
        //    _logger.Log(LogLevel.Information, "Some string!");

        //    using (var activity = new ActivitySource("OpenTelemetryDemo")
        //        .StartActivity("SampleController.Get"))
        //    {
        //        // Додавання атрибутів
        //        activity?.AddTag("http.method", "GET");
        //        activity?.AddTag("custom.attribute", "example_value");

        //        // Додавання атрибутів із контексту запиту
        //        var requestId = HttpContext?.TraceIdentifier;
        //        activity?.SetTag("request_id", requestId);

        //        return Ok(new { Message = "Hello, OpenTelemetry!" });
        //    }
        //}


        // 5
        //public IActionResult Index()
        //{
        //    _logger.Log(LogLevel.Information, "Some string!");

        //    using (var activity = new ActivitySource("OpenTelemetryDemo")
        //    .StartActivity("SampleController.Get"))
        //    {
        //        // Додавання атрибутів
        //        activity?.SetTag("http.method", "GET");
        //        activity?.SetTag("priority", "high");

        //        activity?.AddBaggage("priority", "high");

        //        // Додаємо атрибути з контексту запиту
        //        var requestId = HttpContext?.TraceIdentifier;
        //        activity?.SetTag("request_id", requestId);
        //        return View();
        //    }
        //}

        public IActionResult NoPage()
        {
            return NotFound();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}