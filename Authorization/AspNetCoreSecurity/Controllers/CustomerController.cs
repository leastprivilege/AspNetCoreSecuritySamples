using AspNetCoreAuthentication.Policies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Authentication.Controllers
{
    public class CustomerController : Controller
    {
        private readonly IAuthorizationService _authz;

        public CustomerController(IAuthorizationService authz)
        {
            _authz = authz;
        }

        public async Task<IActionResult> Manage()
        {
            var customer = new Customer
            {
                Name = "Acme Corp",
                Region = "south",
                Fortune500 = true
            };

            var result = await _authz.AuthorizeAsync(User, customer, CustomerOperations.Manage);

            if (result.Succeeded)
            {
                return View("success");
            }

            return Forbid();
        }

        public async Task<IActionResult> Discount(int amount)
        {
            var customer = new Customer
            {
                Name = "Acme Corp",
                Region = "south",
                Fortune500 = true
            };

            var result = await _authz.AuthorizeAsync(User, customer, CustomerOperations.GiveDiscount(amount));

            if (result.Succeeded)
            {
                return View("success");
            }

            return Forbid();
        }
    }
}