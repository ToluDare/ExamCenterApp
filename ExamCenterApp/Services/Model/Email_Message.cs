using System.ComponentModel.DataAnnotations;

namespace ExamCenterApp.Services.Model
{
    public class Email_Message
    {
        public Email_Message()
        {
            TO_addresses = new List<EmailAddress>();

        }
        [EmailAddress(ErrorMessage="Invalid Email Address")]
        public List<EmailAddress> TO_addresses { get; set; }
        public string subject { get; set; }
        public string content { get; set; }
        public IFormFileCollection attachments { get; set; }
    }
}

