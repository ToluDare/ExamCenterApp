using ExamCenterApp.Helpers;
using ExamCenterApp.Models;
using ExamCenterApp.Services;
using ExamCenterApp.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using Org.BouncyCastle.Security;
using System.Text;

namespace ExamCenterApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<Application_Users> _userManager;
        private readonly SignInManager<Application_Users> _signinManager;
        private readonly IUser_Helper _user_helper;
        private readonly IEmail_Sender _email_sender;
        public AccountController(UserManager<Application_Users> userManager, IUser_Helper user_helper, IEmail_Sender email_sender, SignInManager<Application_Users> signinManager)
        {
            _userManager = userManager;
            _user_helper = user_helper;
            _email_sender = email_sender;
            _signinManager = signinManager;
        }
        public IActionResult Register()
        {

            
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(Register_ViewModel model)
        {
            if (model == null){
                return View();
            }
            var user = new Application_Users {
                first_name = model.first_name,
                last_name = model.last_name,
                Email = model.email,
                UserName = model.email,
               
            };
            var invigilator_pass = _user_helper.GeneratePassword();
            var result = await _userManager.CreateAsync(user, invigilator_pass).ConfigureAwait(false);
            if (result.Succeeded)
            {
            //Note: CREATE ROLE
                var add_user_to_role = await _userManager.AddToRoleAsync(user, "ExamInvigilator").ConfigureAwait(false);
                if (add_user_to_role.Succeeded)
                {
                    string link_to_click = HttpContext.Request.Scheme.ToString() + "://" + HttpContext.Request.Host.ToString() + "/Account/Login";
                    string subject = "Exam Supervisor Invitation";
                    string email_address = model.email;

                    //NOTE: Format the Email

                    var messageBuilder = new StringBuilder();
                    messageBuilder.AppendLine($"Hey {user.first_name},");
                    messageBuilder.AppendLine();
                    messageBuilder.AppendLine("You have been invited to be an Exam Invigilator at the MacEwan Exam Center.");
                    messageBuilder.AppendLine("Visit the website or click the link: " + link_to_click + ", and input your login details.");
                    messageBuilder.AppendLine();
                    messageBuilder.AppendLine("Email: " + model.email);
                    messageBuilder.AppendLine("Password: " + invigilator_pass);
                    messageBuilder.AppendLine();
                    messageBuilder.AppendLine("You can change your password later.");
                    messageBuilder.AppendLine("Do not share your details with anyone!");
                    string message = messageBuilder.ToString();


                    if (model.email != null)
                    {
                        _email_sender.SendEmail(email_address, subject, message);

                    }
                }
                else
                {
                    // TempData message = "Exam Invigilator has not been created successfully";
                }

            }
            else
            {
                // TempData message = "Exam Invigilator has not been created successfully";
            }

            return View(model);
            
        }

        //GET
        [AllowAnonymous]

        public IActionResult Login() 
        {
            
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(Login_ViewModel obj )
        {
            var user = new  Application_Users();
            if (obj !=null)
            {
                user = _userManager.FindByEmailAsync(obj.Email).Result;
                if(user == null)
                {
                    return NotFound(); //make as a temp data
                }
                var result = await _signinManager.PasswordSignInAsync(user, obj.Password, true, true).ConfigureAwait(false);
                
                if (result.Succeeded)
                {
                    var url = Url.Action("Privacy", "Home");
                    return Redirect(url);
                }
            }
            return View();
        }

    }
}
