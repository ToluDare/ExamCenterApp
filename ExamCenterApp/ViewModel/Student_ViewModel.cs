using ExamCenterApp.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ExamCenterApp.ViewModel
{
    public class Student_ViewModel
    {
     
        public int id { set; get; }
        public string user_id { set; get; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string exam_course { get; set; }
        public TimeSpan exam_duration { get; set; }
        public DateTime exam_start_time { get; set; }
        public bool is_present { get; set; }
        public string teacher_name { get; set; }
        public string teacher_email { get; set; }
        public string? additional_notes { get; set; }
    }
    public class Student_Model
    {
        public IFormFile student_excel_sheet  { set; get; }
        public List<Student_ViewModel> students { get; set; }
    }
}

