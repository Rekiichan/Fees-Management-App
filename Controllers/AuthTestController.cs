using FeeCollectorApplication.DataAccess;
using FeeCollectorApplication.Models;
using FeeCollectorApplication.Models.ViewModel;
using FeeCollectorApplication.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Linq;
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

        [AllowAnonymous]
        [HttpGet("me")]
        public async Task<IActionResult> TestRole()
        {
            string UserId;
            try
            {
                UserId = User.Claims.FirstOrDefault(u => u.Type == "id").Value;
            }
            catch
            {
                return BadRequest("This user hasn't authenticated");
            }

            var ApplicationUser = await _db.User.FirstOrDefaultAsync(u => u.Id == UserId);
            var UserRoles = await _userManager.GetRolesAsync(ApplicationUser);

            UserVM userVM = new UserVM()
            {
                Id = ApplicationUser.Id,
                Name = ApplicationUser.Name,
                Email = ApplicationUser.Email,
                PhoneNumber = ApplicationUser.PhoneNumber,
                Role = UserRoles.ToList()
            };
            return Ok(userVM);
        }

        [HttpGet("check-authentication")]
        public IActionResult CheckAuthentication()
        {
            return Ok("authencation success!");
        }
        [Authorize(Roles = SD.Role_Admin)]
        [HttpGet("check-is-admin")]
        public IActionResult CheckAdmin()
        {
            return Ok("role admin");
        }
        [Authorize(Roles = SD.Role_Employee)]
        [HttpGet("check-is-employee")]
        public IActionResult CheckEmp()
        {
            return Ok("Role Employee");
        }
        [Authorize(Roles = SD.Role_Customer)]
        [HttpGet("check-is-customer")]
        public IActionResult CheckCustomer()
        {
            return Ok("Role customer");
        }
    }
}
