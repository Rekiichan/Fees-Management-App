using FeeCollectorApplication.Models;
using FeeCollectorApplication.Utility;
using FeeCollectorApplication.Models.DtoModel;
using FeeCollectorApplication.Repository.IRepository;
using FeeCollectorApplication.Models.Dto;
using FeeCollectorApplication.Service.IService;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Security.Claims;
using Models.Models.Dto;


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
            var checkIsExist = userFromDb.FirstOrDefault(u => u.NormalizedEmail == model.Email.ToUpper());
            if (checkIsExist != null)
            {
                ModelState.AddModelError("error", "người dùng này đã tồn tại, vui lòng thử lại");
                return BadRequest(ModelState);
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
                    ModelState.AddModelError("error", "lỗi xảy ra liên quan đến email hoặc username, vui lòng kiểm tra lại");
                    return BadRequest(ModelState);
                }
                if (result.Succeeded)
                {
                    if (!_roleManager.RoleExistsAsync(SD.Role_Admin).GetAwaiter().GetResult())
                    {
                        await _roleManager.CreateAsync(new IdentityRole(SD.Role_Admin));
                    }
                    await _userManager.AddToRoleAsync(newAdmin, SD.Role_Admin);
                    return Ok("tạo tài khoản admin thành công");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            ModelState.AddModelError("error", "vui lòng kiểm tra lại thông tin");
            return BadRequest(ModelState);
        }

        [Authorize(Roles = SD.Role_Admin)]
        [HttpPost("register/employee")]
        public async Task<IActionResult> RegisterEmployee(EmployeeResponse empRequest)
        {
            ApplicationUser userFromDb = await _unitOfWork.ApplicationUser.GetFirstOrDefaultAsync(u => u.NormalizedEmail == empRequest.Email.ToUpper()
            || empRequest.citizenIdentification == u.citizenIdentification);
            if (userFromDb != null)
            {
                ModelState.AddModelError("error", "Email hoặc CCCD/CMND này đã được sử dụng, vui lòng thử cách khác");
                return BadRequest(ModelState);
            }
            ApplicationUser newEmployee = new()
            {
                UserName = empRequest.Email.ToLower().Split('@').First(),
                Email = empRequest.Email,
                NormalizedEmail = empRequest.Email.ToUpper(),
                Name = empRequest.Name,
                PhoneNumber = empRequest.PhoneNumber,
                citizenIdentification = empRequest.citizenIdentification
            };
            if (newEmployee.citizenIdentification.Length != 12 || newEmployee.citizenIdentification.Length != 9)
            {
                ModelState.AddModelError("error", "CCCD/CMND không hợp lệ");
            }
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
                    ModelState.AddModelError("error", "lỗi xảy ra liên quan đến email hoặc username, vui lòng kiểm tra lại");
                    return BadRequest(ModelState);
                }
                if (result.Succeeded)
                {
                    if (!_roleManager.RoleExistsAsync(SD.Role_Employee).GetAwaiter().GetResult())
                    {
                        await _roleManager.CreateAsync(new IdentityRole(SD.Role_Employee));
                    }
                    await _userManager.AddToRoleAsync(newEmployee, SD.Role_Employee);
                    
                    SendMailRegisterEmployee(newEmployee, FirstPassword);
                    var employeeRemove = await _unitOfWork.EmployeeRequest.GetFirstOrDefaultAsync(u => u.Email.ToLower() == newEmployee.Email.ToLower());
                    _unitOfWork.EmployeeRequest.Remove(employeeRemove);
                    await _unitOfWork.Save();
                    return Ok("employee registered");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            ModelState.AddModelError("error", "vui lòng kiểm tra lại thông tin");
            return BadRequest(ModelState);
        }

        [Authorize(Roles = SD.Role_Admin)]
        [HttpPost("reject/employee")]
        public async Task<IActionResult> RejectEmployee(EmployeeResponse empRequest)
        {
            var employeeReject = await _unitOfWork.EmployeeRequest.GetFirstOrDefaultAsync(u => u.Email.ToLower() == empRequest.Email.ToLower());
            if (employeeReject == null)
            {
                ModelState.AddModelError("error", "Nhân viên này không tồn tại, vui lòng kiểm tra lại");
                return BadRequest(ModelState);
            }
            _unitOfWork.EmployeeRequest.Remove(employeeReject);
            await _unitOfWork.Save();
            return Ok();
        }

        [AllowAnonymous]
        [HttpPost("register/customer")] //For Customer
        public async Task<IActionResult> RegisterCustomer([FromBody] CustomerRegisterRequestDTO model)
        {
            var userName = model.Email.ToLower().Split('@').First();
            ApplicationUser userFromDb = await _unitOfWork.ApplicationUser.GetFirstOrDefaultAsync(u => u.UserName.ToLower() == userName.ToLower());
            if (userFromDb != null)
            {
                ModelState.AddModelError("error", "email hoặc tên đăng nhập đã tồn tại, vui lòng sử dụng cái khác");
                return BadRequest(ModelState);
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
                    ModelState.AddModelError("error", "lỗi xảy ra liên quan đến email hoặc username, vui lòng kiểm tra lại");
                    return BadRequest(ModelState);
                }
                if (result.Succeeded)
                {
                    if (!_roleManager.RoleExistsAsync(SD.Role_Customer).GetAwaiter().GetResult())
                    {
                        await _roleManager.CreateAsync(new IdentityRole(SD.Role_Customer));
                    }
                    await _userManager.AddToRoleAsync(newUser, SD.Role_Customer);
                    return Ok("tạo tài khoản thành công");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            ModelState.AddModelError("error", "tạo tài khoản thất bại, vui lòng kiểm tra lại");
            return BadRequest(ModelState);
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
                ModelState.AddModelError("error", "tên đăng nhập hoặc mật khẩu bị sai");
                return BadRequest(ModelState);
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
                ModelState.AddModelError("error login", "tên đăng nhập hoặc mật khẩu bị sai");
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

        private void SendMailRegisterEmployee(ApplicationUser employee, string password)
        {
            EmailDto email = new EmailDto();
            email.Subject = "Chúc mừng trở thành nhân viên của hệ thống thuphigiaothong.com";
            email.Body = $"Đây là tài khoản của bạn:\n" +
                $"Email: {employee.Email}\n" +
                $"Tên đăng nhập: {employee.UserName}\n" +
                $"Mật khẩu: {password}\n" +
                $"\nVui lòng đổi mật khẩu ngay lập tức đề phòng người lạ truy cập bằng mật khẩu khởi tạo này";
            email.ToName = employee.Name;
            email.ToEmailAddress = employee.Email;
            _emailService.SendMail(email);
        }

        bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return false;
            }
            try
            {
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
