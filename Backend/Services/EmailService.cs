using FeeCollectorApplication.Models.Dto;
using FeeCollectorApplication.Services.IService;
using MimeKit.Text;
using MimeKit;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using System.Net.Mail;

namespace FeeCollectorApplication.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;
        public EmailService(IConfiguration config)
        {
            _config = config;
        }
        public void sendEmail(EmailDto emailRequest)
        {
            //var email = new MimeMessage();
            //email.Sender = new MailboxAddress(_config.GetValue<string>("EmailDto:EmailUserName"), _config.GetValue<string>("EmailDto")
            //email.From.Add(MailboxAddress.Parse("virgil53@ethereal.email"));
            //email.To.Add(MailboxAddress.Parse(emailRequest.To));
            //email.Subject = "test email";
            //email.Body = new TextPart(TextFormat.Text) { Text = emailRequest.Body };

            //using var smtp = new SmtpClient();
            //smtp.Connect(_config.GetValue<string>("EmailDto:EmailHost"), 587, MailKit.Security.SecureSocketOptions.StartTls);
            //smtp.Authenticate(_config.GetValue<string>("EmailDto:EmailUserName"), _config.GetValue<string>("EmailDto:EmailPassword"));
            //smtp.Send(email);
            //smtp.Disconnect(true);


            // Set up the email message
            MailMessage message = new MailMessage("dtnhan.ete.dut@gmail.com", "nhan9655@gmail.com", "subject",  "asadasda");
            //message.To.Add("nhan9655@gmail.com");
            //message.Subject = "Subject of the EmailDto";
            //message.Body = "Body oasdasdasf the EmailDto";

            // Set up the SMTP client
            System.Net.Mail.SmtpClient smtpClient = new System.Net.Mail.SmtpClient("smtp.gmail.com");
            smtpClient.Port = 587; // or 25, depending on your SMTP provider
            smtpClient.Credentials = new System.Net.NetworkCredential("dtnhan.ete.dut@gmail.com", "Nailghan8091.");
            smtpClient.EnableSsl = true; // enable SSL encryption for secure communication

            // Send the email
            smtpClient.Send(message);
        }
    }
}
