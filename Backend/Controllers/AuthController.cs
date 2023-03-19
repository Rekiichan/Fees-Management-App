using FeeCollectorApplication.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using FeeCollectorApplication.Utility;
using FeeCollectorApplication.Models.DtoModel;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using FeeCollectorApplication.Models.Dto;
using FeeCollectorApplication.Repository.IRepository;
using FeeCollectorApplication.Services.IService;
using FeeCollectorApplication.Services;
using System.Net;

namespace FeeCollectorApplication.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly EmailHostedService _emailService;
        private string secretKey;
        public AuthController(IUnitOfWork db, IConfiguration options,
            UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager,
            EmailHostedService emailService)
        {
            _unitOfWork = db;
            secretKey = options.GetValue<string>("ApiSettings:Secret");
            _userManager = userManager;
            _roleManager = roleManager;
            _emailService = emailService;
        }

        //[Authorize(Roles = SD.Role_Admin)]
        [AllowAnonymous] // TODO: just remove in future
        [HttpPost("register/admin")]
        public async Task<IActionResult> Register([FromBody] RegisterAdminRequestDTO model)
        {
            ApplicationUser userFromDb = await _unitOfWork.ApplicationUser.GetFirstOrDefaultAsync(u => u.Email.ToLower() == model.Email.ToLower());
            if (userFromDb != null)
            {
                return BadRequest("This userName has already existed!");
            }

            ApplicationUser newAdmin = new()
            {
                UserName = model.Name.Replace(" ", ""),
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

        [Authorize(Roles = SD.Role_Admin)]
        [HttpPost("register/employee")]
        public async Task<IActionResult> RegisterEmployee(EmployeeRequest empRequest, bool isAccept)
        {
            if (isAccept)
            {
                ApplicationUser newEmployee = new()
                {
                    UserName = empRequest.Name.Replace(" ", ""),
                    Email = empRequest.Email,
                    NormalizedEmail = empRequest.Email.ToUpper(),
                    Name = empRequest.Name,
                    PhoneNumber = empRequest.PhoneNumber,
                    citizenIdentification = empRequest.citizenIdentification
                };
                string FirstPassword = "abcde12345"; // call email service
                try
                {
                    IdentityResult result;
                    try
                    {
                        result = await _userManager.CreateAsync(newEmployee, FirstPassword);
                    }
                    catch (Exception)
                    {
                        return BadRequest("Problem occurs when assign username or full name! Please re-check again!");
                    }
                    if (result.Succeeded)
                    {
                        if (!_roleManager.RoleExistsAsync(SD.Role_Employee).GetAwaiter().GetResult())
                        {
                            await _roleManager.CreateAsync(new IdentityRole(SD.Role_Employee));
                        }
                        await _userManager.AddToRoleAsync(newEmployee, SD.Role_Employee);
                        return Ok("Employee registered");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
                return BadRequest("please re-check the information");
            }

            else
            {
                // TODO
                return Ok();
            }
        }

        [AllowAnonymous]
        [HttpPost("register/customer")] //For Customer
        public async Task<IActionResult> RegisterCustomer([FromBody] CustomerRegisterRequestDTO model)
        {
            string userName = model.Name.Replace(" ", "");
            ApplicationUser userFromDb = await _unitOfWork.ApplicationUser.GetFirstOrDefaultAsync(u => u.UserName.ToLower() == userName.ToLower());
            if (userFromDb != null)
            {
                return BadRequest("This userName has already exist!!!");
            }

            ApplicationUser newUser = new ApplicationUser()
            {
                UserName = userName,
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
            return BadRequest("Failed to register new userName!");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO model)
        {
            ApplicationUser userFromDb = await _unitOfWork.ApplicationUser.GetFirstOrDefaultAsync(u => u.Email.ToLower() == model.Email.ToLower());
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

        [AllowAnonymous]
        [HttpPost("send-email")]
        public async Task<IActionResult> SendEmail(EmailDto emailRequest)
        {
            await _emailService.SendEmailAsync(new EmailDto
            {
                EmailAddress = "nhantrongnt123@gmail.com",
                Subject = "string",
                Body = WebUtility.HtmlDecode("string")
            });
            return (Ok("Sent"));
        }
    }
}
