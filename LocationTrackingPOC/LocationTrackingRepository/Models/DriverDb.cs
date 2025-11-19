using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocationTrackingRepository.Models
{
    [Table("driver")]

    public class DriverDb
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }

        [Required]
        [Column("user_id")]
        public long UserId { get; set; }

        [Column("vehicle_number")]
        [StringLength(50)]
        public string VehicleNumber { get; set; }

        [Column("license_number")]
        [StringLength(50)]
        public string LicenseNumber { get; set; }

        [Column("created_date")]
        public DateTime CreatedDate { get; set; }

        [Column("created_by")]
        public long CreatedBy { get; set; }

        [Column("updated_date")]
        public DateTime? UpdatedDate { get; set; }

        [Column("updated_by")]
        public long? UpdatedBy { get; set; }

        // Navigation property to User
        [ForeignKey("UserId")]
        public virtual UserDb User { get; set; }
        [ForeignKey(nameof(CreatedBy))]
        public virtual UserDb CreatedByUser { get; set; }

        [ForeignKey(nameof(UpdatedBy))]
        public virtual UserDb UpdatedByUser { get; set; }
    }
}
