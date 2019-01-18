using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace Api
{
    [Route("test")]
    public class TestController : ControllerBase
    {
        public IActionResult Get()
        {
            return new JsonResult("OK");
        }

        [Route("identity")]
        [Authorize]
        public IActionResult Identity()
        {
            var claims = from c in User.Claims select new { c.Type, c.Value };

            return new JsonResult(claims);
        }
    }
}