using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocationTrackingRepository.Models
{
    [Table("collection_request")]

    public class CollectionRequestDb
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }

        [Required]
        [Column("user_id")]
        public long UserId { get; set; }

        [Column("driver_id")]
        public long? DriverId { get; set; }

        [Required]
        [Column("pickup_location_id")]
        public long PickupLocationId { get; set; }

        [Required]
        [Column("status")]
        // Change this as well to int for better performance
        // Provide a separate ride_statuses table for extensibility
        public int Status { get; set; }

        [Required]
        [Column("requested_at")]
        public DateTime RequestedAt { get; set; }

        [Column("assigned_at")]
        public DateTime? AssignedAt { get; set; }

        [Column("completed_at")]
        public DateTime? CompletedAt { get; set; }

        [Column("created_date")]
        public DateTime CreatedDate { get; set; }

        [Column("created_by")]
        public long CreatedBy { get; set; }

        [Column("updated_date")]
        public DateTime? UpdatedDate { get; set; }

        [Column("updated_by")]
        public long? UpdatedBy { get; set; }

        // Navigation properties
        [ForeignKey("UserId")]
        public virtual UserDb User { get; set; }

        [ForeignKey("DriverId")]
        public virtual DriverDb? Driver { get; set; }
        [ForeignKey("PickupLocationId")]
        public virtual AddressDb PickupLocation { get; set; }
        [ForeignKey("Status")]
        public virtual CollectionStatusDb CollectionStatus { get; set; }
    }
}
