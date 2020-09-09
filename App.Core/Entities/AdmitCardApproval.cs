using System;

namespace App.Core.Entities
{
    public class AdmitCardApproval
    {
        public long Id { get; set; }

        public StudentInfo StudentInfo { get; set; }
        public long StudentInfoId { get; set; }

        public Payment Payment { get; set; }
        public long? PaymentId { get; set; }

        public bool IsPaymentComplete { get; set; }

        public bool IsSpecialPermission { get; set; }
        public DateTime? ExceptedDate { get; set; }
        public string Comments { get; set; }

        public bool IsPrevious { get; set; }


        public bool IsDelete { get; set; }
        public string ApproveBy { get; set; }
        public DateTime ApproveDate { get; set; }
        public string DeleteBy { get; set; }
        public DateTime? DeleteDate { get; set; }
    }
}