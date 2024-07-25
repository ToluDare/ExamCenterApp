using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExamCenterApp.Models
{
    public class Student
    {
        [Key]
        public int id {set; get; }
        public string user_id {set; get; }
        [ForeignKey("user_id")]
        public Application_Users user { get; set; }

        public string exam_course { get; set; }
        public TimeSpan exam_duration {  get; set; }
        public DateTime exam_start_time { get; set; }
        public bool is_present { get; set; }
        public string teacher_name { get; set; }
        public string teacher_email { get; set; }
        public string? additional_notes { get; set; }
    }
}
