using FeeCollectorApplication.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FeeCollectorApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AuthTestController : ControllerBase
    {
        [HttpGet]
        public IActionResult actionResultGet()
        {
            return Ok("authencation success!");
        }
        [Authorize(Roles = SD.Role_Admin)]
        [HttpGet("{id:int}")]
        public IActionResult actionResultPost(int id = 1)
        {
            return Ok("role admin");
        }
    }
}
