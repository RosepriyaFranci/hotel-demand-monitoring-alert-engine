namespace XORevenueEngine.Models
{
    public class Booking
    {
        public int Id { get; set; }

        public DateTime BookingDate { get; set; }

        public DateTime CheckinDate { get; set; }

        public int Rooms { get; set; }

        public int Nights { get; set; }

        public string Source { get; set; } = "";

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}