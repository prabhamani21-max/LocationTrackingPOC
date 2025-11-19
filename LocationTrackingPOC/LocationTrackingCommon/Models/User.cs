
using System.Data;

namespace LocationTrackingCommon.Models
{
    public class User
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public int StatusId { get; set; }
        public int Gender { get; set; }
        public int RoleId { get; set; }
        public DateOnly DOB { get; set; }
        public string ContactNo { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public long CreatedBy { get; set; }
        public long? UpdatedBy { get; set; }
        public virtual UserStatus Status { get; set; }
       public virtual Role Role { get; set; }
    }
}
