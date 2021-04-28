using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebApplication1.DataAccess.Interfaces;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ILogRepository _cloudLogger;
        public HomeController(ILogger<HomeController> logger, ILogRepository cloudLogger)
        {
            _cloudLogger = cloudLogger;
            _logger = logger;
        }

        public IActionResult Index()
        {

            _logger.LogInformation("log somethng locally");
            
            _cloudLogger.Log("User accessed Index page", Google.Cloud.Logging.Type.LogSeverity.Info);

           
          //      throw new Exception("Error thrown on purpose");
          

            return View();
        }

        public IActionResult Privacy()
        {
            throw new Exception("unhandled exception");
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
