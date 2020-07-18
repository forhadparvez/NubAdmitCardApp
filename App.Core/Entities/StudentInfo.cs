using System;

namespace App.Core.Entities
{
    public class StudentInfo
    {
        public long Id { get; set; }

        public Program Program { get; set; }
        public byte ProgramId { get; set; }

        public Semester Semester { get; set; }
        public int SemesterId { get; set; }

        public Exam Exam { get; set; }
        public byte ExamId { get; set; }

        public string IdNo { get; set; }
        public string Name { get; set; }

        public string ContactNo { get; set; }
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