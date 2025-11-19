using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocationTrackingRepository.Models
{
    [Table("user")]

    public class UserDb
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }

        [Required, MaxLength(100)]
        [Column("name")]
        public string Name { get; set; }

        [Required, MaxLength(320)]
        [Column("email")]
        public string Email { get; set; }

        [Required, MaxLength(2000)]
        [Column("password")]
        public string Password { get; set; }

        [Column("status_id")]
        public int StatusId { get; set; }
        [Column("role_id")]
        public int RoleId { get; set; }
        [Column("gender")]
        public int Gender { get; set; }

        [Column("dob", TypeName = "date")]
        public DateOnly DOB { get; set; }

        [MaxLength(15)]
        [Column("contact_no")]
        public string ContactNo { get; set; }

        [Column("created_date")]
        public DateTime CreatedDate { get; set; }
        [Column("updated_date")]
        public DateTime? UpdatedDate { get; set; }

        [Column("created_by")]
        public long CreatedBy { get; set; }
        [Column("updated_by")]
        public long? UpdatedBy { get; set; }

        // 🔗 Foreign key to Status table
        [ForeignKey(nameof(StatusId))]
        public virtual UserStatusDb Status { get; set; }

        // 🔗 Foreign key to Role table
        [ForeignKey(nameof(RoleId))]
        public virtual RoleDb Role { get; set; }

        //  Self-referencing relationships
        [ForeignKey(nameof(CreatedBy))]
        public virtual UserDb CreatedByUser { get; set; }

        [ForeignKey(nameof(UpdatedBy))]
        public virtual UserDb UpdatedByUser { get; set; }
    }
}
