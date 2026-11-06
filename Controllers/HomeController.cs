using AromaDom.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace AromaDom.Controllers
{
    public class HomeController : Controller
    {
        
        public IActionResult Index()
        {
            return RedirectToAction("Index", "Materials");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
