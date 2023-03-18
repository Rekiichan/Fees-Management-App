using FeeCollectorApplication.Models.Dto;

namespace FeeCollectorApplication.Services.IService
{
    public interface IEmailService
    {
        void sendEmail(EmailDto emailRequest);
    }
}
