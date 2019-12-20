using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

namespace BlazorServer
{
    public class LogoutModel : PageModel
    {
        public async Task OnGet()
        {
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                await HttpContext.SignOutAsync("oidc");
            }
            else
            {
                Response.Redirect("/");
            }
        }
    }
}