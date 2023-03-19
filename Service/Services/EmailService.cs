using FeeCollectorApplication.Models.Dto;
using FeeCollectorApplication.Services.IService;
using Mailjet.Client;
using Newtonsoft.Json.Linq;

namespace FeeCollectorApplication.Services
{
    public abstract class EmailService : IEmailService
    {
        public static MailjetClient CreateMailJetV3Client()
        {
            return new MailjetClient("32ac4dfc1a965524dbf7dc5d033a77bb", "eeb49672c2ab62522bb5d4812c8b6ff8");
        }
        protected abstract Task Send(EmailDto email);
        public Task SendEmail(string address, string subject, string text)
        {
            return SendEmail(new EmailDto { EmailAddress = address, Subject = subject, Body = text });
        }

        public Task SendEmail(EmailDto email)
        {
            return Send(email);
        }
    }
    public class MailJetService : EmailService, IEmailService
    {
        protected override async Task Send(EmailDto email)
        {
            try
            {
                JArray jArray = new JArray();
                JArray attachments = new JArray();
                if (email.Attachments != null && email.Attachments.Count() > 0)
                {

                    email.Attachments.ToList().ForEach(attachment =>
                    attachments.Add(
                        new JObject {
                            new JProperty("Content-type",attachment.ContentType),
                            new JProperty( "Filename",attachment.Name),
                            new JProperty("content",Convert.ToBase64String(attachment.Data))
                    }));
                }

                jArray.Add(new JObject {
                            {
                            "FromEmail",
                            "dtnhan.ete.dut@gmail.com"
                            }, {
                            "FromName",
                            "Nhan Dao"
                            }, {
                            "Recipients",
                            new JArray {
                                new JObject {
                                {
                                    "Email",
                                    email.EmailAddress
                                }, {
                                    "Name",
                                   email.EmailAddress
                                }
                                }
                            }
                            }, {
                            "Subject",
                            email.Subject
                            }, {
                            "Text-part",
                            email.Body
                            }, {
                            "Html-part",
                            email.Body
                            },  {
                            "Attachments",
                            attachments
                            }});

                var client = CreateMailJetV3Client();
                MailjetRequest request = new MailjetRequest
                {
                    Resource = Mailjet.Client.Resources.Send.Resource,
                }
                 .Property(Mailjet.Client.Resources.Send.Messages, jArray);

                var response = await client.PostAsync(request);
                Console.WriteLine(response.StatusCode);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
