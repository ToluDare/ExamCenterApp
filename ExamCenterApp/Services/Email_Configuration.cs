namespace ExamCenterApp.Services
{
    public class Email_Configuration : IEmail_Configuration
    {
        public string SmtpServer { get; set; }

        public string SmtpUsername { get; set; }

        public int SmtpPort { get; set; }
        public string SmtpPassword { get; set; }
        public string DisplayName { get; set; }
    }
    public interface IEmail_Configuration
    {
        string SmtpServer { get; }
        int SmtpPort { get; }
        string SmtpUsername { get; set; }
        string SmtpPassword { get; set; }
        string DisplayName { get; set; }
    }

}
