using ExamCenterApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ExamCenterApp.Database
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
        public DbSet<Application_Users> Application_Users { get; set; }
        public DbSet<Exam_Session> Exam_Sessions { get; set; }
        public DbSet<Student> Student { get; set; }
    }
}
        //protected override void OnModelCreating(ModelBuilder builder)
        //{
        //    base.OnModelCreating(builder);         // Configure the many-to-many relationship between users and roles
        //    builder.Entity<IdentityUserRole<string>>()  
        //        .HasKey(ur => new { ur.UserId, ur.RoleId });
        //    builder.Entity<IdentityUserRole<string>>() 
        //        //.HasOne(ur => ur.User) 
        //        .WithMany(u => u.UserRoles)
        //        .HasForeignKey(ur => ur.UserId) 
        //        .IsRequired();
        //    builder.Entity<IdentityUserRole<string>>() 
        //        .HasOne(ur => ur.Role) 
        //        .WithMany(r => r.UserRoles) 
        //        .HasForeignKey(ur => ur.RoleId) 
        //        .IsRequired(); }
        //}

