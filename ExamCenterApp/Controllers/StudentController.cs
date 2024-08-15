using ExamCenterApp.Database;
using ExamCenterApp.Models;
using ExamCenterApp.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using NuGet.Packaging.Signing;
using OfficeOpenXml;

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
                first_name = student.first_name,
                last_name = student.last_name,
                exam_course = student.exam_course,
                exam_duration = student.exam_duration,
                exam_start_time = student.exam_start_time,
                exam_end_time = student.exam_end_time,
                is_present= student.is_present,
                teacher_name= student.teacher_name,
                teacher_email = student.teacher_email,
                additional_notes = student.additional_notes
            }).ToList();
            model.students= list_of_student.OrderBy(u => u.exam_start_time).ToList();

            return View(model);
        }
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Student_Model obj)
        {
            if (obj.student_excel_sheet ==null || obj.student_excel_sheet.Length == 0)
            {
                //Temp Data - The file is not valid - empty
                return View(obj);
            }
            var list_of_students = new List<Student_ViewModel>();
            using (var stream= new MemoryStream())
            {
                obj.student_excel_sheet.CopyTo(stream);
                using(var package = new ExcelPackage(stream))
                {
                    var worksheet =package.Workbook.Worksheets[0];
                    var rowCounts = worksheet.Dimension.Rows;
                    for (int row = 2;  row <= rowCounts;  row++)
                    {
                        if (IsRowEmpty(worksheet, row)) {
                            continue;
                        };
                        var convert_time = worksheet.Cells[row, 4].Text;
                        DateTime exam_time ;
                        TimeSpan exam_duration = TimeSpan.Zero;
                        if(DateTime.TryParse(convert_time,out exam_time))
                        {
                            exam_duration = exam_time.TimeOfDay;

                        }
                        var student_info = new Student_ViewModel
                        {
                            first_name = worksheet.Cells[row, 1].Text,
                            last_name = worksheet.Cells[row,2].Text,
                            exam_course = worksheet.Cells[row,3].Text,
                            exam_duration = exam_duration,
                            exam_start_time= DateTime.Parse(worksheet.Cells[row,5].Text),
                            exam_end_time= DateTime.Parse(worksheet.Cells[row,6].Text),
                            teacher_name= worksheet.Cells[row,7].Text,
                            teacher_email= worksheet.Cells[row,8].Text
                        };

                        IsColumnEmpty(student_info);// edit the spreadsheet with an empty column.
                        list_of_students.Add(student_info);

                    }
                }
            }
            var list = list_of_students.Select(m => new Student
            {
                first_name= m.first_name,
                last_name= m.last_name,
                exam_course= m.exam_course,
                exam_duration= m.exam_duration,
                exam_start_time= m.exam_start_time,
                exam_end_time = m.exam_end_time,
                teacher_name= m.teacher_name,
                teacher_email= m.teacher_email
            }).ToList();
            _applicationDbContext.Student.AddRange(list);
            await _applicationDbContext.SaveChangesAsync();
            return View(obj);
        }
        public void IsColumnEmpty(Student_ViewModel studentInfo)
        //method that checks if any of the columns are null, if it is, it sets a default value ORRR
        //                                                                                          give a notification and then highlight the row as red in the dashboard to edit. 
        {
            var defaultValues = new Dictionary<string, object>
        {
            { "first_name", "....." },
            { "last_name", "....." },
            { "exam_course", "....." },
            { "exam_duration", TimeSpan.FromHours(1) },
            { "exam_start_time", DateTime.Now },
            { "exam_end_time", DateTime.Now.AddHours(1) },
            { "teacher_name", "Mr(s)....." },
            { "teacher_email", "teacher@example.com" },
            { "additional_notes", "......." }
        };

            foreach (var property in typeof(Student_ViewModel).GetProperties())
            {
                var propertyName = property.Name;
                var propertyValue = property.GetValue(studentInfo);

                if (property.PropertyType == typeof(string) && string.IsNullOrEmpty(propertyValue as string))
                {
                    property.SetValue(studentInfo, defaultValues[propertyName]);
                }
                else if (property.PropertyType == typeof(TimeSpan) && (TimeSpan)propertyValue == TimeSpan.Zero)
                {
                    property.SetValue(studentInfo, defaultValues[propertyName]);
                }
                else if (property.PropertyType == typeof(DateTime) && (DateTime)propertyValue == default(DateTime))
                {
                    property.SetValue(studentInfo, defaultValues[propertyName]);
                }
            }
        }

        private bool IsRowEmpty(ExcelWorksheet worksheet, int row) {
            for (int col = 1; col <= worksheet.Dimension.Columns; col++)
            { 
                if (!string.IsNullOrEmpty(worksheet.Cells[row, col].Text)) 
                { return false; }
            }
            return true;
        }
        public IActionResult Edit(int id)
        {
            var db = new Student();
            if (id > 0)
            {
                db = _applicationDbContext.Student.FirstOrDefault(u => u.id == id);
            }
            return View(db );
        }

        [HttpPost]
        public IActionResult Edit(Student obj)

        {
            if(obj != null)
            {
                var db = _applicationDbContext.Student.FirstOrDefault(u => u.id == obj.id);
                if (db != null)
                {
                    db.first_name = obj.first_name;
                    db.last_name = obj.last_name;
                    db.exam_course = obj.exam_course;
                    db.exam_start_time = obj.exam_start_time;
                    db.exam_duration = obj.exam_duration;
                    db.exam_end_time = obj.exam_start_time.Add(obj.exam_duration);
                    db.teacher_name = obj.teacher_name;
                    db.teacher_email = obj.teacher_email;
                    db.additional_notes = obj.additional_notes;
                }
                _applicationDbContext.Student.Update(db);
                _applicationDbContext.SaveChanges();
            }
            return RedirectToAction("Student_Index");
        }
    
        [HttpPost]
        public IActionResult Delete(int id)
        {
            if (id > 0)
            {
                var db = _applicationDbContext.Student.FirstOrDefault(u => u.id == id);
                if (db != null)
                {
                    _applicationDbContext.Remove(db);
                    _applicationDbContext.SaveChanges();
                    return RedirectToAction("Student_Index");
                }
            }
            return NotFound();
        }

       
    }
}
