namespace XORevenueEngine.DTOs
{
    public class BookingUploadDto
    {
        public string booking_date { get; set; } = "";

        public string checkin_date { get; set; } = "";

        public int rooms { get; set; }

        public int nights { get; set; }

        public string source { get; set; } = "";
    }
}