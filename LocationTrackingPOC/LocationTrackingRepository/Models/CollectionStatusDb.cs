using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LocationTrackingRepository.Models
{
    [Table("collection_status")]

    public class CollectionStatusDb
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("name")]
        public string Name { get; set; }

        [Required]
        [Column("is_active", TypeName = "boolean")]
        public bool IsActive { get; set; }

        [Required]
        [Column("created_date")]
        public DateTime CreatedDate { get; set; }

        [Column("created_by")]
        public long? CreatedBy { get; set; }

        [Column("updated_by")]
        public long? UpdatedBy { get; set; }

        [Column("updated_date")]
        public DateTime? UpdatedDate { get; set; }
        // 🔁 Self-referencing relationships
        [ForeignKey(nameof(CreatedBy))]
        public virtual UserDb CreatedByUser { get; set; }

        [ForeignKey(nameof(UpdatedBy))]
        public virtual UserDb? UpdatedByUser { get; set; }
    }
}
