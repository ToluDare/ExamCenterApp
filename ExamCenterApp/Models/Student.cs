using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExamCenterApp.Models
{
    public class Student
    {
        public Student()
        {
           
            additional_notes = string.Empty;
        }
        [Key]
        public int id {set; get; }
        public string first_name { get; set; }
        public string last_name { get; set; }

        public string exam_course { get; set; }
        public TimeSpan exam_duration {  get; set; }
        public DateTime exam_start_time { get; set; }
        public DateTime exam_end_time { get; set; }
        public string teacher_name { get; set; }
        public string teacher_email { get; set; }
        public bool is_present { get; set; }
        public string? additional_notes { get; set; }

    }
}
