using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Mvc;
using System.Threading.Tasks;

namespace Authentication.Controllers
{
    public class TestController : Controller
    {
        private readonly IAuthorizationService _authz;

        public TestController(IAuthorizationService authz)
        {
            _authz = authz;
        }

        [Authorize("SalesOnly")]
        public IActionResult SalesOnly()
        {
            return View("success");
        }

        [Authorize("SalesSenior")]
        public IActionResult SalesSenior()
        {
            return View("success");
        }

        public async Task<IActionResult> SalesSeniorImperative()
        {
            if (await _authz.AuthorizeAsync(User, "SalesSenior"))
            {
                return View("success");
            }

            return new ChallengeResult();
        }

        [Authorize("DevInterns")]
        public IActionResult DevInterns()
        {
            return View("success");
        }

        [Authorize(Roles = "NoOneHasAccess")]
        public IActionResult NoAccess()
        {
            return View("success");
        }
    }
}