namespace LocationTrackingPOC.DTO
{
    public class CollectionRequestDto
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public long? DriverId { get; set; }
        public long PickupLocationId { get; set; }
        public LocationTrackingCommon.Models.CollectionStatusEnum Status { get; set; }
        public DateTime RequestedAt { get; set; }
        public DateTime? AssignedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public DateTime CreatedDate { get; set; }
        public long CreatedBy { get; set; }
    }
}