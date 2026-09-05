using System;

namespace XORevenueEngine.Models
{
    public class Alert
    {
        public int Id { get; set; }

        public string AlertDate { get; set; }

        public string AlertType { get; set; }

        public string Severity { get; set; }

        public string Recommendation { get; set; }

        public string Status { get; set; }
            = "Open";

        public string CreatedAt
        {
            get;
            set;
        }
    }
}