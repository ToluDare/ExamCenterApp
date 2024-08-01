namespace ExamCenterApp.Services
{
    public interface IEmail_Sender
    {
        void SendEmail(string TO_email, string subject, string message, string user_display_name = null);
        void SendEmailWithAttachment(string TO_email, string subject, string message, IFormFileCollection attachments, string user_display_name = null);
    }
}
