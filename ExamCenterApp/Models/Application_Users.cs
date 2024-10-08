﻿using ExamCenterApp.Enums;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.ExceptionServices;

namespace ExamCenterApp.Models
{
    public class Application_Users: IdentityUser
    {
        public Application_Users()
        {
            date_user_created= DateTime.Now;
            status = User_Status.active;
            guid = Guid.NewGuid().ToString();
            user_roles = new HashSet<IdentityRole<string>>();
        }
      
        public string? first_name { get; set; }
        public string? last_name { get; set; }
        public User_Status status { get; set; }
        public DateTime date_user_created { get; set; }
        public string? guid { get; set; }
        public DateTime last_login_date { get; set; }
        [NotMapped]
        public virtual ICollection<IdentityRole<string>> user_roles { get; set;}
    }
}
