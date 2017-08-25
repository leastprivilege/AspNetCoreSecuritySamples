using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.DataProtection;

namespace AspNetCoreAuthentication.Controllers
{
    public class HomeController : Controller
    {
        private IDataProtector _protector;

        public HomeController(IDataProtectionProvider provider)
        {
            _protector = provider.CreateProtector(GetType().FullName);
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Protect(Data data)
        {
            data.CipherText = _protector.Protect(data.PlainText);

            return View("index", data);
        }

        public IActionResult Unprotect(Data data)
        {
            data.PlainText = _protector.Unprotect(data.CipherText);

            return View("index", data);
        }
    }

    public class Data
    {
        public string PlainText { get; set; }
        public string CipherText { get; set; }
    }
}