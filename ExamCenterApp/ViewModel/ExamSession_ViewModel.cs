using ExamCenterApp.Models;

namespace ExamCenterApp.ViewModel
{
    public class ExamSession_ViewModel
    {
        public int Id { get; set; }
        public int[] Ids { get; set; }
        public string returnUrl { get; set; }
        public string startTime { get; set; }
        public string? date { get; set; }
        public string? location { get; set; }
        public string Invigilators_Id { get; set; }
    } 
}
