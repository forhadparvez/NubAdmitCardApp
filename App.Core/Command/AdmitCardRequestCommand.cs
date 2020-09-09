using App.Core.Entities;
using System.ComponentModel.DataAnnotations;

namespace App.Core.Command
{
    public class AdmitCardRequestCommand
    {
        [Display(Name = "Student ID")]
        public string IdNo { get; set; }

        [Display(Name = "Apply Permission Date")]
        public string RequestedDate { get; set; }

        [Display(Name = "Tentative Payment Date with Reason")]
        public string Comment { get; set; }


        public Semester Semester { get; set; }
        [Display(Name = "Semester")]
        public int SemesterId { get; set; }

        public Exam Exam { get; set; }
        [Display(Name = "Exam")]
        public byte ExamId { get; set; }
    }
}