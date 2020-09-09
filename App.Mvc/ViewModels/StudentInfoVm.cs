using App.Core.Entities;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace App.Mvc.ViewModels
{
    public class StudentInfoVm
    {
        public long Id { get; set; }

        public Program Program { get; set; }
        [Display(Name = "Program")]
        public byte ProgramId { get; set; }

        //public Semester Semester { get; set; }
        //[Display(Name = "Semester")]
        //public int SemesterId { get; set; }

        //public Exam Exam { get; set; }
        //[Display(Name = "Exam")]
        //public byte ExamId { get; set; }

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

        [Required]
        [Display(Name = "Student Photo")]
        public HttpPostedFileBase StudentImageFile { get; set; }

        //[Required]
        //[Display(Name = "Payment Screenshot")]
        //public HttpPostedFileBase StudentPaymentFile { get; set; }

        [Required]
        public string Captcha { get; set; }

        [Required]
        public string WhoIs { get; set; }
    }
}