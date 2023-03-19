using FeeCollectorApplication.Models.Dto;

namespace FeeCollectorApplication.Service.IService
{
    public interface IEmailService
    {
        Task SendMail(EmailDto emailDto);
    }
}
