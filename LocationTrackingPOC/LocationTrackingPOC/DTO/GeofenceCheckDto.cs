namespace LocationTrackingPOC.DTO
{
    public class GeofenceCheckDto
    {
        public long DriverId { get; set; }
        public long TargetLocationId { get; set; } // User location for pickup, facility for drop-off
        public double BufferMeters { get; set; } // 20 for pickup, 50 for drop-off
        public string CheckType { get; set; } // "pickup" or "dropoff"
    }
}