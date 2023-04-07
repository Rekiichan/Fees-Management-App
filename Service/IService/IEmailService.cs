using FeeCollectorApplication.Models.Dto;

namespace FeeCollectorApplication.Service.IService
{
    public interface IMailService
    {
        Task<bool> SendAsync(EmailDto mailData, CancellationToken ct);
    }
}
