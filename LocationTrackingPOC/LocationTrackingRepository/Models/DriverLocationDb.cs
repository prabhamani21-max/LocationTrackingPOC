using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocationTrackingRepository.Models
{
    [Table("driver_location")]
    public class DriverLocationDb
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }

        [Required]
        [Column("driver_id")]
        public long DriverId { get; set; }

        [Required]
        [Column("location")]
        public Point Location { get; set; }
        // Change this to int for better performance
        // Create a separate driver_statuses table for extensibility    
        [Column("status")]
        public int Status { get; set; }

        [Column("timestamp")]
        public DateTime Timestamp { get; set; }

        [Column("created_date")]
        public DateTime CreatedDate { get; set; }

        [Column("created_by")]
        public long CreatedBy { get; set; }
        public long? UpdatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }
        // 🔁 Self-referencing relationships
        [ForeignKey(nameof(CreatedBy))]
        public virtual UserDb CreatedByUser { get; set; }

        [ForeignKey(nameof(UpdatedBy))]
        public virtual UserDb UpdatedByUser { get; set; }
        // Navigation property
        [ForeignKey("DriverId")]
        public virtual DriverDb? Driver { get; set; }
        [ForeignKey("Status")]
        public virtual DriverStatusDb? DriverStatus { get; set; }

    }
}
