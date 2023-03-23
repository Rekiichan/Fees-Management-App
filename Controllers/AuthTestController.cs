using FeeCollectorApplication.DataAccess;
using FeeCollectorApplication.Models;
using FeeCollectorApplication.Models.ViewModel;
using FeeCollectorApplication.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FeeCollectorApplication.Controllers
{
    [Route("api/auth-test")]
    [ApiController]
    [Authorize]
    public class AuthTestController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        public AuthTestController(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        private UserVM getUserVM (string role)
        {
            var name = User.Claims.First().Value.ToString();
            var model = _db.User.FirstOrDefault(u => u.Name == name);
            UserVM userVM = new UserVM()
            {
                Id = model.Id,
                Name = name,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                Role = role
            };
            return userVM;
        }

        [AllowAnonymous]
        [HttpGet("me")]
        public async Task<IActionResult >TestRole()
        {
            var isAdmin = User.HasClaim(c => c.Type == ClaimTypes.Role && c.Value == SD.Role_Admin);
            if (isAdmin)
            {
                return Ok(getUserVM(SD.Role_Admin));
            }
            var isEmployee = User.HasClaim(c => c.Type == ClaimTypes.Role && c.Value == SD.Role_Employee);
            if (isEmployee)
            {
                return Ok(getUserVM(SD.Role_Employee));
            }
            else 
            {
                return Ok(getUserVM(SD.Role_Customer));
            }
        }

        [HttpGet("CheckAuthentication")]
        public IActionResult CheckAuthentication()
        {
            return Ok("authencation success!");
        }
        [Authorize(Roles = SD.Role_Admin)]
        [HttpGet("CheckIsAdmin")]
        public IActionResult CheckAdmin()
        {
            return Ok("role admin");
        }
        [Authorize(Roles = SD.Role_Employee)]
        [HttpGet("CheckIsEmployee")]
        public IActionResult CheckEmp()
        {
            return Ok("Role Employee");
        }
        [Authorize(Roles = SD.Role_Customer)]
        [HttpGet("CheckIsCustomer")]
        public IActionResult CheckCustomer()
        {
            return Ok("Role customer");
        }
    }
}
