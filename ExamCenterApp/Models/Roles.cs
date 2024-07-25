using Microsoft.AspNetCore.Identity;

namespace ExamCenterApp.Models
{
    public class Roles : IdentityRole
    {
        public Roles() : base() { }
        public Roles(string name) : base(name) { }
        public virtual ICollection<IdentityRole<string>> user_roles {get; set;}
    }
}
