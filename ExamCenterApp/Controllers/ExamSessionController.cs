using ExamCenterApp.Database;
using ExamCenterApp.Models;
using ExamCenterApp.ViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Linq;

namespace ExamCenterApp.Controllers
{
    public class ExamSessionController : Controller
    {
        private ApplicationDbContext _db;
        private readonly UserManager<Application_Users> _userManager;
        public ExamSessionController(ApplicationDbContext db, UserManager<Application_Users> userManager)
        {
            _db =  db;
            _userManager = userManager;

        }

        public IActionResult ExamSession_Index()
        {
            var session = _db.Exam_Sessions.Include(u => u.students).Include(u => u.User).OrderByDescending(u => u.date_created).ThenBy(u => u.start_date_time).Select(ab => new ExamSession_ViewModel
            {
               Id= ab.Id,
               location=ab.location,
               DateTime = ab.start_date_time,
               students = ab.students,
               invigilators_name = ab.User.first_name + " " + ab.User.last_name
             


            }).ToList();
            return View(session);
        }
        public async Task<IActionResult> ExamSesssion_Create(int[] ids, string returnUrl)
        {
           

            var invigilators = _userManager.GetUsersInRoleAsync("ExamInvigilator").Result;

            if (invigilators.Count == 0)
            {
                //temp data - action cannot be done because there are no invigilators
                returnUrl = Url.Action("Student_index", "Student");
                
                return Redirect(returnUrl);
                
            }

            IEnumerable<SelectListItem> Invigilators = invigilators.Select(u => new SelectListItem
            {
                Text = u.first_name + " " + u.last_name,
                Value = u.Id.ToString()

            }).ToList(); 

            var model = new ExamSession_ViewModel();

            if (ids == null)
            {
                //return "sorry but you havent selected any";
                if (!string.IsNullOrEmpty(returnUrl))
                {
                 return  Redirect(returnUrl);
                }
                return RedirectToAction("Index");

            }
            model.Ids= ids;
            model.returnUrl = returnUrl;
            var exam_session_details = _db.Student.Where(u => ids.Contains(u.id)).OrderBy(u => u.exam_start_time).ToList();
            var anything = exam_session_details.FirstOrDefault();
            model.startTime = anything.exam_start_time.ToString("hh:mm tt");
            model.date = anything.exam_start_time.Date.ToString("MMM dd");
            ViewBag.Invigilators = Invigilators;
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ExamSession_Create (ExamSession_ViewModel obj)
        {
          
            if(obj.location == null || obj.Invigilators_Id == null)
            {
                if (!string.IsNullOrEmpty(obj.returnUrl))
                {
                    return Redirect(obj.returnUrl);
                }
                return RedirectToAction("Index");
            }
            var date_string = $"{obj.date}{obj.startTime}";
            var date_string_format = "MMM. dd hh:mm tt";
            var Exam_Session = new Exam_Session {
                Invigilators_Id = obj.Invigilators_Id,
                location = obj.location,
                start_date_time = DateTime.ParseExact(date_string, date_string_format,CultureInfo.InvariantCulture),
                students= new List<Student>()

            };

            var invigilators_email = _db.Application_Users.Where(u => u.UserName == obj.invigilators_name);//then get email

            Exam_Session.students = _db.Student.Where(u => obj.Ids.Contains(u.id)).ToList();
            _db.Exam_Sessions.Add(Exam_Session);
            _db.SaveChangesAsync();
           
            return RedirectToAction("Index");

        }
    }
}
