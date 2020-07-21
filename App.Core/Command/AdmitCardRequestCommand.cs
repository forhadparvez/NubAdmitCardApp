namespace App.Core.Command
{
    public class AdmitCardRequestCommand
    {
        public string IdNo { get; set; }

        public string RequestedDate { get; set; }
        public string Comment { get; set; }
    }
}