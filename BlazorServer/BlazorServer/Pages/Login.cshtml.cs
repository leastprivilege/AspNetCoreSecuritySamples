using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

namespace BlazorServer
{
    public class LoginModel : PageModel
    {
        public async Task OnGet()
        {
            if (!HttpContext.User.Identity.IsAuthenticated)
            {
                await HttpContext.ChallengeAsync(new AuthenticationProperties { RedirectUri = "/secure" });
            }
            else
            {
                Response.Redirect("/");
            }
        }
    }
}