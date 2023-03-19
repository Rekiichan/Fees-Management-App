using FeeCollectorApplication.Models.Dto;

namespace FeeCollectorApplication.Services.IService
{
    public interface IEmailService
    {
        Task SendEmail(string address, string subject, string text);
        Task SendEmail(EmailDto email);
    }
}
