using DataProtectionDemo.ViewModels;
using Microsoft.AspNet.DataProtection;
using Microsoft.AspNet.Mvc;

namespace DataProtectionDemo.Controllers
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
}