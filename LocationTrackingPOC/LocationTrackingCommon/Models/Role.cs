
namespace LocationTrackingCommon.Models
{
    public class Role
    {

        public int Id { get; set; }


        public string RoleName { get; set; }

        public int StatusId { get; set; }

        public DateTime CreatedDate { get; set; }

        public long CreatedBy { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public long? ModifiedBy { get; set; }
    }
}