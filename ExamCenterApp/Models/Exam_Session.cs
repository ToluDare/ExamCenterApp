namespace ExamCenterApp.Models
{
    public class Exam_Session
    {
        public int Id { get; set; }
        public DateTime start_date_time { get; set; }
        public string location { get; set; }
        public ICollection<Student> students { get; set; }
    }
}
