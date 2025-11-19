using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LocationTrackingCommon.Models
{
    public class Driver
    {
   
        public long Id { get; set; }

     
        public string Name { get; set; }

 
        public string Email { get; set; }

        public string ContactNo { get; set; }

        public int StatusId { get; set; }

        public string VehicleNumber { get; set; }

        public string LicenseNumber { get; set; }

        public DateTime CreatedDate { get; set; }

        public long CreatedBy { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public long? ModifiedBy { get; set; }
        public virtual User User { get; set; }
    }
}