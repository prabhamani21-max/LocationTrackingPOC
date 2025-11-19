namespace LocationTrackingPOC.DTO
{
    public class DriverLocationDto
    {
        public long Id { get; set; }
        public long DriverId { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public DateTime Timestamp { get; set; }
        public int Status { get; set; } // e.g., "online", "offline"
    }
}