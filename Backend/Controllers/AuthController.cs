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
using FeeCollectorApplication.Repository.IRepository;
using FeeCollectorApplication.Models.Dto;
using Models.Models.Dto;
using FeeCollectorApplication.Service.IService;

namespace FeeCollectorApplication.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailService _emailService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private string secretKey;
        public AuthController(IUnitOfWork db, IConfiguration options,
            UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager,
            IEmailService emailService)
        {
            _unitOfWork = db;
            secretKey = options.GetValue<string>("ApiSettings:Secret");
            _userManager = userManager;
            _roleManager = roleManager;
            _emailService = emailService;
        }

        #region register account
        [Authorize(Roles = SD.Role_Admin)]
        //[AllowAnonymous] // TODO: just remove in future
        [HttpPost("register/admin")]
        public async Task<IActionResult> Register([FromBody] RegisterAdminRequestDTO model)
        {
            var userFromDb = await _unitOfWork.ApplicationUser.GetAllAsync();
            var checkIsExist = userFromDb.FirstOrDefault(u => u.Name.ToLower() == model.Name.ToLower() || u.Email.ToLower() == model.Email.ToLower());
            if (checkIsExist != null)
            {
                return BadRequest("this username has already existed!");
            }

            ApplicationUser newAdmin = new()
            {
                UserName = model.Email.ToLower().Split('@').First(),
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
                    return BadRequest("problem occurs when assign username or full name, please re-check again");
                }
                if (result.Succeeded)
                {
                    if (!_roleManager.RoleExistsAsync(SD.Role_Admin).GetAwaiter().GetResult())
                    {
                        await _roleManager.CreateAsync(new IdentityRole(SD.Role_Admin));
                    }
                    await _userManager.AddToRoleAsync(newAdmin, SD.Role_Admin);
                    return Ok("admin registered");
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
                    UserName = empRequest.Email.ToLower().Split('@').First(),
                    Email = empRequest.Email,
                    NormalizedEmail = empRequest.Email.ToUpper(),
                    Name = empRequest.Name,
                    PhoneNumber = empRequest.PhoneNumber,
                    citizenIdentification = empRequest.citizenIdentification
                };
                string FirstPassword = "abcde12345"; // call Email service
                try
                {
                    IdentityResult result;
                    try
                    {
                        result = await _userManager.CreateAsync(newEmployee, FirstPassword);
                    }
                    catch (Exception)
                    {
                        return BadRequest("problem occurs when assign username or full name! please re-check again!");
                    }
                    if (result.Succeeded)
                    {
                        if (!_roleManager.RoleExistsAsync(SD.Role_Employee).GetAwaiter().GetResult())
                        {
                            await _roleManager.CreateAsync(new IdentityRole(SD.Role_Employee));
                        }
                        await _userManager.AddToRoleAsync(newEmployee, SD.Role_Employee);
                        return Ok("employee registered");
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
                return Ok("reject this employee");
            }
        }

        [AllowAnonymous]
        [HttpPost("register/customer")] //For Customer
        public async Task<IActionResult> RegisterCustomer([FromBody] CustomerRegisterRequestDTO model)
        {
            var userName = model.Email.ToLower().Split('@').First();
            ApplicationUser userFromDb = await _unitOfWork.ApplicationUser.GetFirstOrDefaultAsync(u => u.UserName.ToLower() == userName.ToLower());
            if (userFromDb != null)
            {
                return BadRequest("this username has already exist!!!");
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
                    return BadRequest("problem occurs when assign username or full name! please re-check again!");
                }
                if (result.Succeeded)
                {
                    if (!_roleManager.RoleExistsAsync(SD.Role_Customer).GetAwaiter().GetResult())
                    {
                        await _roleManager.CreateAsync(new IdentityRole(SD.Role_Customer));
                    }
                    await _userManager.AddToRoleAsync(newUser, SD.Role_Customer);
                    return Ok("registered");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return BadRequest("failed to register new username!");
        }

        #endregion

        #region login account
        [AllowAnonymous]
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
                ModelState.AddModelError("error login", "username or password is incorrect");
                return BadRequest(ModelState);
            }
            return Ok(loginResponse);
        }

        #endregion

        #region password process

        [AllowAnonymous]
        [HttpPost("forgot-password-request")]
        public async Task<IActionResult> ForgotPasswordRequest(ForgotPassword model)
        {
            if (IsValidEmail(model.Email))
            {
                var user = await _unitOfWork.ApplicationUser.GetFirstOrDefaultAsync(x => x.Email == model.Email);
                if (user == null)
                {
                    return BadRequest("this Email has not been already exist");
                }

                EmailDto emailRequest = new EmailDto()
                {
                    ToName = user.Name,
                    ToEmailAddress = model.Email,
                    Subject = "Please reset password",
                    Body = $"Hi {user.Name},\r\nWe received a request to reset your Thuphigiaothong.com password.\r\nPlease click this Link: {model.Link} to reset your password\r\nAlternatively, you can directly change your password."
                };

                await _emailService.SendMail(emailRequest);
                return Ok("sent");
            }
            return BadRequest($"{model.Email} is an invalid Email address");
        }

        [AllowAnonymous]
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPassword model)
        {
            var applicationUser = await _unitOfWork.ApplicationUser.GetFirstOrDefaultAsync(u => u.Email.ToLower() == model.Email.ToLower());
            if (applicationUser == null)
            {
                return BadRequest("cant find this user");
            }

            var result = await _userManager.ChangePasswordAsync(applicationUser, model.OldPassword, model.NewPassword);
            if (result.Succeeded)
            {
                return Ok("password changed");
            }
            
            return BadRequest("password changes fail");
        }

        #endregion

        #region process function
        bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return false;
            }
            try
            {
                // Use the built-in MailAddress class to validate the Email format
                var addr = new System.Net.Mail.MailAddress(email);
                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion
    }
}
