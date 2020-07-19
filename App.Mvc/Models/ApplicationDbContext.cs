using App.Core.Entities;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;

namespace App.Mvc.Models
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public DbSet<Exam> Exams { get; set; }
        public DbSet<Program> Programs { get; set; }
        public DbSet<Semester> Semesters { get; set; }


        public DbSet<StudentInfo> StudentInfos { get; set; }
    }
}