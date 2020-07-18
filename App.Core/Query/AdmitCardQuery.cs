namespace App.Core.Query
{
    public class AdmitCardQuery
    {
        public long Id { get; set; }

        public string Program { get; set; }
        public string Semester { get; set; }
        public string Exam { get; set; }


        public string IdNo { get; set; }
        public string Name { get; set; }

        public string ContactNo { get; set; }
        public string Email { get; set; }

        public byte[] StudentImage { get; set; }
        public byte[] Qr { get; set; }
    }
}