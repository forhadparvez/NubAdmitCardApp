using System;
using System.Collections.Generic;

namespace App.Core.Entities
{
    public class Semester
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Year { get; set; }
        public bool IsActive { get; set; }

        public string AddBy { get; set; }
        public DateTime AddDate { get; set; }
        public string EditBy { get; set; }
        public DateTime? EditDate { get; set; }


        public List<string> GetAll()
        {
            var r = new List<string> { "Spring", "Summer", "Fall" };
            return r;
        }
    }
}