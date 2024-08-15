using ExamCenterApp.Database;
using ExamCenterApp.Models;
using ExamCenterApp.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ExamCenterApp.Controllers
{
    public class ExamSessionController : Controller
    {
        private ApplicationDbContext _db;
        public ExamSessionController(ApplicationDbContext db)
        {
            _db =  db;

        }

        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> ExamSesssion_Create(int[] ids, string returnUrl)
        {
            IEnumerable<SelectListItem> Invigilators = _db.Application_Users.Select(u => new SelectListItem
            {
                Text = u.first_name + " " + u.last_name,
                Value = u.Id.ToString()

            }); 

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
            var Exam_Session = new Exam_Session {
                Invigilators_Id = obj.Invigilators_Id,
                location = obj.location,
                start_date_time= DateTime.Parse($"{obj.date}{obj.startTime}"),
                students= new List<Student>()

            };
            Exam_Session.students = _db.Student.Where(u => obj.Ids.Contains(u.id)).ToList();
            _db.Exam_Sessions.Add(Exam_Session);
            _db.SaveChangesAsync();
            return RedirectToAction("Index");

        }
    }
}
