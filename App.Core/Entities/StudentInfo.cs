using System;
using System.ComponentModel.DataAnnotations;

namespace App.Core.Entities
{
    public class StudentInfo
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
        [Display(Name = "ID No")]
        public string IdNo { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Contact No")]
        public string ContactNo { get; set; }

        [Required]
        public string Email { get; set; }

        public string ImageFilePath { get; set; }
        public string PaymentFilePath { get; set; }


        public bool IsDelete { get; set; }
        public string EditBy { get; set; }
        public DateTime? EditDate { get; set; }
        public string DeleteBy { get; set; }
        public DateTime? DeleteDate { get; set; }
    }
}