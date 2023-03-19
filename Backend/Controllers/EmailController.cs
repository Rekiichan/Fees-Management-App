using FeeCollectorApplication.Models;
using FeeCollectorApplication.Models.Dto;
using FeeCollectorApplication.Repository.IRepository;
using FeeCollectorApplication.Service.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Models.Models.Dto;

namespace FeeCollectorApplication.Controllers
{
    [Route("api/Email")]
    [Authorize]
    public class EmailController : ControllerBase
    {
        private readonly IEmailService _emailService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        public EmailController(IEmailService emailService, IUnitOfWork unitOfWork,
            UserManager<ApplicationUser> userManager)
        {
            _emailService = emailService;
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        [AllowAnonymous]
        [HttpPost("forgot-password")]
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
                    Body = $"Hi {user.Name},\r\nWe received a request to reset your Thuphigiaothong.com password.\r\nPlease click this link: {model.Email} to reset your password\r\nAlternatively, you can directly change your password."
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
            var result = await _userManager.ChangePasswordAsync(applicationUser, model.OldPassword, model.NewPassword);
            if (result.Succeeded)
            {
                return Ok("password changed");
            }
            return BadRequest("password changes fail");
        }

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
