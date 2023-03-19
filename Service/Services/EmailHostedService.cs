using FeeCollectorApplication.Models.Dto;
using FeeCollectorApplication.Services;
using FeeCollectorApplication.Services.IService;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks.Dataflow;

namespace FeeCollectorApplication.Services
{
    public class EmailHostedService : IHostedService, IDisposable
    {
        private Task? _sendTask;
        private CancellationTokenSource? _cancellationTokenSource;
        private readonly BufferBlock<EmailDto> _mailQueue;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IEmailService _mailSender;
        public EmailHostedService(IServiceScopeFactory serviceScopeFactory)
        {
            _mailSender = new MailJetService(); ;
            _serviceScopeFactory = serviceScopeFactory;
            _cancellationTokenSource = new CancellationTokenSource();
            _mailQueue = new BufferBlock<EmailDto>();
        }

        public async Task SendEmailAsync(EmailDto emailWithAddress)
        {
            await _mailQueue.SendAsync(emailWithAddress);
            Console.WriteLine($"SEND {emailWithAddress.EmailAddress}");
        }

        public void Dispose()
        {
            DestroyTask();
        }
        private void DestroyTask()
        {
            try
            {
                if (_cancellationTokenSource != null)
                {
                    _cancellationTokenSource.Cancel();
                    _cancellationTokenSource = null;
                }
                Console.WriteLine("[EMAIL HOSTED SERVICE] DESTROY SERVICE");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _sendTask = BackgroundSendEmailAsync(_cancellationTokenSource!.Token);
            return Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            DestroyTask();
            await Task.WhenAny(_sendTask!, Task.Delay(Timeout.Infinite, cancellationToken));
        }
        private async Task BackgroundSendEmailAsync(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                try
                {
                    var email = await _mailQueue.ReceiveAsync();
                    await _mailSender.SendEmail(email);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[BACKGROUND EMAIL SERVICE] {ex.Message}", "EmailHostedService");
                }
                Console.WriteLine("[BACKGROUND EMAIL SERVICE] END SEND", "EmailHostedService");
            }
        }
    }
}
