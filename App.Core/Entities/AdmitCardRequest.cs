using System;

namespace App.Core.Entities
{
    public class AdmitCardRequest
    {
        public long Id { get; set; }
        public StudentInfo StudentInfo { get; set; }
        public long StudentInfoId { get; set; }

        public DateTime RequestedDate { get; set; }
        public string Comment { get; set; }

        public bool Status { get; set; }
        public bool IsDone { get; set; }
    }
}