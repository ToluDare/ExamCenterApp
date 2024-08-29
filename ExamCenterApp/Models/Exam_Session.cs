using System.ComponentModel.DataAnnotations.Schema;

namespace ExamCenterApp.Models
{
    public class Exam_Session
    {
        public int Id { get; set; }
        public string Invigilators_Id { get; set; }
        [ForeignKey("Invigilators_Id")]
        public virtual Application_Users User { get; set; }
        public DateTime start_date_time { get; set; }
        public string location { get; set; }
        public ICollection<Student> students { get; set; }
        public DateTime date_created { get; set; } = DateTime.Now;
        
    }
}
