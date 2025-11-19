
using NetTopologySuite.Geometries;

namespace LocationTrackingCommon.Models
{
    public class DriverLocation
    {
    
        public long Id { get; set; }

        public long DriverId { get; set; }

       public Point Location { get; set; }

        public DateTime Timestamp { get; set; }

        public DateTime CreatedDate { get; set; }

        public long CreatedBy { get; set; }

        public virtual Driver? Driver { get; set; }
    }
}