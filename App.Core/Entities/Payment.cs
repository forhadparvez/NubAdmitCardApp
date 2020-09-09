using System.ComponentModel.DataAnnotations;

namespace App.Core.Entities
{
    public class Payment
    {
        public Program Program { get; set; }
        public byte ProgramId { get; set; }

        public Semester Semester { get; set; }
        public int SemesterId { get; set; }

        public Exam Exam { get; set; }
        public byte ExamId { get; set; }

        public long Id { get; set; }
        public string StudentId { get; set; }
        public string StudentName { get; set; }
        public decimal WaiverPercent { get; set; }
        public decimal CreditTaken { get; set; }
        public decimal NetPayable { get; set; }
        public decimal PreviousDues { get; set; }
        public decimal TotalPayable { get; set; }
        public decimal PayablePercentAmount { get; set; }
        public decimal ReceivedAmount { get; set; }
        public decimal DuesPercentAmount { get; set; }
        public decimal TotalDues { get; set; }

        public bool IsDelete { get; set; }
    }
}