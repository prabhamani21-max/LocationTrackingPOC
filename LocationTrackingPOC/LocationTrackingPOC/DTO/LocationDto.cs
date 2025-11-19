namespace LocationTrackingPOC.DTO
{
    public class LocationDto
    {
        public long Id { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string? FullAddress { get; set; } // Optional full address text
        public string Label { get; set; }
        public long UserId { get; set; }
    }
}
