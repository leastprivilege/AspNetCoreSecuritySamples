using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Authorization;

namespace AspNet5Authentication.Controllers
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

        public IActionResult Error()
        {
            return View();
        }
    }
}
