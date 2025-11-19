using NetTopologySuite.Geometries;
using System.ComponentModel.DataAnnotations.Schema;


namespace LocationTrackingCommon.Models
{
    public class Address
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public string Label { get; set; } // Home, Work, etc.
        public string? FullAddress { get; set; }
       public Point Location { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public long CreatedBy { get; set; }
        public long? UpdatedBy { get; set; }
    }
}
