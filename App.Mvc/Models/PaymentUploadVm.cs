using App.Core.Entities;
using System.ComponentModel.DataAnnotations;
using System.Web;

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


        [Display(Name = "Min Due Payment")]
        public decimal MinDuePayment { get; set; }

        [Display(Name = "Max Due Payment")]
        public decimal MaxDuePayment { get; set; }

        [Display(Name = "Permission Date")]
        public string PermissionDate { get; set; }

        public HttpPostedFileBase File { get; set; }
    }
}