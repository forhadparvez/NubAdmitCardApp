using System.ComponentModel.DataAnnotations;

namespace App.Core.Query
{
    public class PaymentStatusApprovalQuery
    {
        public long Id { get; set; }

        [Display(Name = "Student ID")]
        public string IdNo { get; set; }
        public string Name { get; set; }

        public string RequestId { get; set; }
        public string RequestedDate { get; set; }
        public string Comment { get; set; }

        public string ImageFilePath { get; set; }
        public string PaymentFilePath { get; set; }

        public string PreviousPermission { get; set; }
    }
}