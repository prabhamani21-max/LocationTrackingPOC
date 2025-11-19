using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocationTrackingRepository.Models {

    [Table("address")]
    public class AddressDb
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }
        [Column("user_id")]
        public long UserId { get; set; }
        [Required, MaxLength(20)]
        [Column("label")]
        public string Label { get; set; } // Home, Work, etc.
        [Column("full_address")]
        public string? FullAddress { get; set; }
        [Column("location")]
        public Point Location { get; set; }
        [ForeignKey(nameof(UserId))]
        [Column("created_date")]
        public DateTime CreatedDate { get; set; }
        [Column("updated_date")]
        public DateTime? UpdatedDate { get; set; }

        [Column("created_by")]
        public long CreatedBy { get; set; }
        [Column("updated_by")]
        public long? UpdatedBy { get; set; }

        public virtual UserDb? User { get; set; }
    [ForeignKey(nameof(CreatedBy))]
    public virtual UserDb CreatedByUser { get; set; }

    [ForeignKey(nameof(UpdatedBy))]
    public virtual UserDb UpdatedByUser { get; set; }
}
}
