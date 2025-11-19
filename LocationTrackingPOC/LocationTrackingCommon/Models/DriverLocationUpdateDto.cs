namespace LocationTrackingCommon.Models
{
    public class DriverLocationUpdateDto
    {
        public long DriverId { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public int Status { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public DateTime LastPing { get; set; } = DateTime.UtcNow;
        public double? LastOfflineLatitude { get; set; }
        public double? LastOfflineLongitude { get; set; }
        public DateTime? LastOfflineTimestamp { get; set; }
    }
}
