using ExamCenterApp.Database;
using ExamCenterApp.Models;
using ExamCenterApp.ViewModel;
using Microsoft.AspNetCore.Mvc;

namespace ExamCenterApp.Controllers
{
    public class StudentController : Controller
    {
        private readonly ApplicationDbContext _applicationDbContext;
        
        public StudentController(ApplicationDbContext applicationDbContext)
        {
           _applicationDbContext = applicationDbContext;
            
        }
        public IActionResult Student_Index()
        {
            var model = new Student_Model();
            //list of students
            var list_of_student = _applicationDbContext.Student.Select(student => new Student_ViewModel
            {
                id = student.id,
                user_id = student.user_id,
                first_name = student.first_name,
                last_name = student.last_name,
                exam_course = student.exam_course,
                exam_duration = student.exam_duration,
                exam_start_time = student.exam_start_time,
                is_present= student.is_present,
                teacher_name= student.teacher_name,
                teacher_email = student.teacher_email,
                additional_notes = student.additional_notes
            }).ToList();
            model.students= list_of_student;

            return View(model);
        }
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Student obj)
        {
            return View();
        }
        public IActionResult Edit()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Edit(Student obj)
        {
            return View();
        }
        public IActionResult Delete()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Delete(Student obj)
        {
            return View();
        }
    }
}
