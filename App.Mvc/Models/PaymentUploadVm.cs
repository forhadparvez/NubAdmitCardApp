using System.ComponentModel.DataAnnotations;
using System.Web;
using App.Core.Entities;

namespace App.Mvc.Models
{
    public class PaymentUploadVm
    {
        public Program Program { get; set; }
        [Display(Name = "Program")]
        public byte ProgramId { get; set; }

        public Semester Semester { get; set; }
        [Display(Name = "Semester")]
        public int SemesterId { get; set; }

        public Exam Exam { get; set; }
        [Display(Name = "Exam")]
        public byte ExamId { get; set; }
        public HttpPostedFileBase File { get; set; }
    }
}