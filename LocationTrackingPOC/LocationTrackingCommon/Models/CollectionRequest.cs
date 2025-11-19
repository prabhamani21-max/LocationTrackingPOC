namespace LocationTrackingCommon.Models
{
    public class CollectionRequest
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public long? DriverId { get; set; }
        public long PickupLocationId { get; set; }
        public CollectionStatusEnum Status { get; set; }
        public DateTime RequestedAt { get; set; }
        public DateTime? AssignedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public DateTime CreatedDate { get; set; }
        public long CreatedBy { get; set; }
        public virtual User User { get; set; }
        public virtual Driver? Driver { get; set; }
        public virtual Address PickupLocation { get; set; }
    }
}