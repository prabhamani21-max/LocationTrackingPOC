using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocationTrackingRepository.Models
{
    public class DropLocationDb
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }

        [Column("driver_id")]
        public long? DriverId { get; set; }
        [Required]
        [Column("collection_request_id")]
        public long CollectionRequestId { get; set; }
        [Required]
        [Column("dump_location_id")]
        public long DumpLocationId { get; set; }

        [Column("completed_at")]
        public DateTime? CompletedAt { get; set; }

        [Column("distance_km")]
        public double? DistanceKm { get; set; }

        [Column("created_date")]
        public DateTime CreatedDate { get; set; }

        [Column("created_by")]
        public long CreatedBy { get; set; }

        [Column("updated_date")]
        public DateTime? UpdatedDate { get; set; }

        [Column("updated_by")]
        public long? UpdatedBy { get; set; }

        [ForeignKey("DriverId")]
        public virtual DriverDb? Driver { get; set; }
        [ForeignKey("CollectionRequestId")]
        public virtual CollectionRequestDb PickupLocation { get; set; }
        [ForeignKey("DumpLocationId")]
        public virtual AddressDb DumpLocation { get; set; }
    }
}
