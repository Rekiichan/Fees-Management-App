using FeeCollectorApplication.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FeeCollectorApplication.Controllers
{
    [Authorize(Policy = SD.Policy_SuperAdmin)]
    [ApiController]
    [Route("api/role")]
    public class RoleManagerController : ControllerBase
    {
        
        public RoleManagerController() { }
        [HttpGet]
        public void GetRolesOfUser()
        {
            var user = User.Claims;
            Console.WriteLine(user);
        }
    }
}
