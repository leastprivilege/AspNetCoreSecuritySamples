using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace AspNetCoreSecurity.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        public IActionResult Secure()
        {
            return View();
        }

        [Authorize(Roles = "foo")]
        public IActionResult Secure2()
        {
            return View();
        }
    }
}
