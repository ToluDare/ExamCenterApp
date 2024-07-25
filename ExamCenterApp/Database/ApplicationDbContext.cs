using ExamCenterApp.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ExamCenterApp.Database
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options): base(options) {}
        public DbSet<Application_Users> Application_Users { get; set; }
        public DbSet<Exam_Session> Exam_Sessions { get; set; }
        public DbSet<Student> Student { get; set; }

    }
}
