namespace LocationTrackingPOC.DTO
{
    public class DriverLocationUpdateDto
    {
        public long DriverId { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public int Status { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}