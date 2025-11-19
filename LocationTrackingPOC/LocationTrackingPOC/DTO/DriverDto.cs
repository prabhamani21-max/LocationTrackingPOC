namespace LocationTrackingPOC.DTO
{
    public class DriverDto
    {
        public long Id { get; set; }
        public string VehicleNumber { get; set; }
        public string LicenseNumber { get; set; }
        public UserDto User { get; set; }

    }
}