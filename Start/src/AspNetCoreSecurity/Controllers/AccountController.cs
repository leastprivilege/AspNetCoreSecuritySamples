using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace AspNetCoreSecurity.Controllers
{
    public class AccountController : Controller
    {
        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string userName, string password, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (!string.IsNullOrWhiteSpace(userName) &&
                userName == password)
            {

            }

            return View();
        }
    }
}