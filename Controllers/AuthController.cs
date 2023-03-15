using FeeCollectorApplication.Models;
using FeeCollectorApplication.DataAccess;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using FeeCollectorApplication.Utility;
using FeeCollectorApplication.Models.DtoModel;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using System.Data;

namespace FeeCollectorApplication.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private string secretKey;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public AuthController(ApplicationDbContext db, IConfiguration options,
            UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _db = db;
            secretKey = options.GetValue<string>("ApiSettings:Secret");
            _userManager = userManager;
            _roleManager = roleManager;
        }

        //[Authorize(Roles = SD.Role_Admin)]
        [HttpPost("register/admin")]
        public async Task<IActionResult> Register([FromBody] RegisterAdminRequestDTO model)
        {
            ApplicationUser userFromDb = _db.User.FirstOrDefault(u => u.UserName.ToLower() == model.UserName.ToLower());
            if (userFromDb != null)
            {
                return BadRequest("This user has already existed!");
            }

            ApplicationUser newAdmin = new()
            {
                UserName = model.UserName,
                Email = model.Email,
                NormalizedEmail = model.Email.ToUpper(),
                Name = model.Name,
                PhoneNumber = model.PhoneNumber
            };
            try
            {
                IdentityResult result;
                try
                {
                    result = await _userManager.CreateAsync(newAdmin, model.Password);
                }
                catch (Exception)
                {
                    return BadRequest("Problem occurs when assign username or full name! Please re-check again!");
                }
                if (result.Succeeded)
                {
                    if (!_roleManager.RoleExistsAsync(SD.Role_Admin).GetAwaiter().GetResult())
                    {
                        await _roleManager.CreateAsync(new IdentityRole(SD.Role_Admin));
                    }
                    await _userManager.AddToRoleAsync(newAdmin, SD.Role_Admin);
                    return Ok("Admin registered");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return BadRequest("please re-check the information");
        }

        

        [AllowAnonymous]
        [HttpPost("register/customer")] //For Customer
        public async Task<IActionResult> RegisterCustomer([FromBody] CustomerRegisterRequestDTO model)
        {
            ApplicationUser userFromDb = _db.User.FirstOrDefault(u => u.Name.ToLower() == model.Name.ToLower());
            if (userFromDb != null)
            {
                return BadRequest("This user has already exist!!!");
            }

            ApplicationUser newUser = new ApplicationUser()
            {
                UserName = model.UserName,
                Email = model.Email,
                NormalizedEmail = model.Email.ToUpper(),
                Name = model.Name,
                PhoneNumber = model.PhoneNumber
            };
            try
            {
                IdentityResult result;
                try
                {
                    result = await _userManager.CreateAsync(newUser, model.Password);
                }
                catch (Exception)
                {
                    return BadRequest("Problem occurs when assign username or full name! Please re-check again!");
                }
                if (result.Succeeded)
                {
                    if (!_roleManager.RoleExistsAsync(SD.Role_Customer).GetAwaiter().GetResult())
                    {
                        await _roleManager.CreateAsync(new IdentityRole(SD.Role_Customer));
                    }
                    await _userManager.AddToRoleAsync(newUser, SD.Role_Customer);
                    return Ok("Registered");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return BadRequest("Failed to register new user!");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO model)
        {
            ApplicationUser userFromDb = _db.User.FirstOrDefault(u => u.UserName.ToLower() == model.UserName.ToLower());
            bool isValid = await _userManager.CheckPasswordAsync(userFromDb, model.Password);
            if (isValid == false)
            {
                return BadRequest(new LoginResponseDTO());
            }
            // if login success, have to generate JWT Token
            var roles = await _userManager.GetRolesAsync(userFromDb);
            JwtSecurityTokenHandler tokenHandler = new();
            byte[] key = Encoding.ASCII.GetBytes(secretKey);
            SecurityTokenDescriptor tokenDescriptor = new()
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim("fullName", userFromDb.Name),
                    new Claim("id", userFromDb.Id.ToString()),
                    new Claim(ClaimTypes.Email, userFromDb.UserName.ToString()),
                    new Claim(ClaimTypes.Role, roles.FirstOrDefault())
                }),
                Expires = DateTime.Now.AddDays(2),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);
            LoginResponseDTO loginResponse = new()
            {
                UserName = userFromDb.UserName,
                Token = tokenHandler.WriteToken(token),
                Id = userFromDb.Id,
                FullName = userFromDb.Name,
                Email = userFromDb.Email,
                PhoneNumber = userFromDb.PhoneNumber
            };
            if (loginResponse.UserName == null || string.IsNullOrEmpty(loginResponse.Token))
            {
                ModelState.AddModelError("error login", "Username or password is incorrect");
                return BadRequest(ModelState);
            }
            return Ok(loginResponse);
        }
    }
}
