using ExamCenterApp.Helpers;
using ExamCenterApp.Models;
using ExamCenterApp.ViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Security;

namespace ExamCenterApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<Application_Users> _userManager;
        private readonly IUser_Helper _user_helper;
        public AccountController(UserManager<Application_Users> userManager, IUser_Helper user_helper)
        {
            _userManager = userManager;
             _user_helper = user_helper;
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
                email = model.email,
                UserName = model.email,
               
            };
            var password = _user_helper.GeneratePassword();
            var result = await _userManager.CreateAsync(user, password).ConfigureAwait(false);
            if (result.Succeeded)
            {
                var add_user_to_role = await _userManager.AddToRoleAsync(user, "EXAM SUPERVISOR").ConfigureAwait(false);
                if (add_user_to_role.Succeeded)
                {

                }
            }

           
        }

    }
}
