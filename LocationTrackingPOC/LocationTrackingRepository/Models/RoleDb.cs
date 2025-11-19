using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocationTrackingRepository.Models
{
    [Table("roles")]

    public class RoleDb
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("role_name")]
        [StringLength(50)]
        public string RoleName { get; set; }

        [Column("status_id")]
        public int StatusId { get; set; }

        [Column("created_date")]
        public DateTime CreatedDate { get; set; }

        [Column("created_by")]
        public long? CreatedBy { get; set; }

        [Column("updated_date")]
        public DateTime? UpdatedDate { get; set; }

        [Column("updated_by")]
        public long? UpdatedBy { get; set; }
        [ForeignKey(nameof(CreatedBy))]
        public virtual UserDb CreatedByUser { get; set; }

        [ForeignKey(nameof(UpdatedBy))]
        public virtual UserDb UpdatedByUser { get; set; }
    }

}
