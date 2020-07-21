using System.ComponentModel.DataAnnotations;
using App.Core.Entities;

namespace App.Core.Query
{
    public class StudentEditVm
    {
        public long Id { get; set; }

        public Program Program { get; set; }
        [Display(Name = "Program")]
        public byte ProgramId { get; set; }

        public Semester Semester { get; set; }
        [Display(Name = "Semester")]
        public int SemesterId { get; set; }

        public Exam Exam { get; set; }
        [Display(Name = "Exam")]
        public byte ExamId { get; set; }

        [Required]
        [Display(Name = "Student ID")]
        public string IdNo { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Contact No")]
        public string ContactNo { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}