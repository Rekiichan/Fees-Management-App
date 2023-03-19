using System.Net.Mail;

namespace FeeCollectorApplication.Models.Dto
{
    public class EmailDto
    {
        public string EmailAddress { get; set; } = String.Empty;
        public string Subject { get; set; } = String.Empty;
        public string Body { get; set; } = String.Empty;
        public IEnumerable<MyAttachment>? Attachments { get; set; }
    }
    public class MyAttachment
    {
        public string ContentType { get; set; } = String.Empty;
        public string Name { get; set; } = String.Empty;
        public byte[] Data { get; set; } = new byte[10];
    }
}
