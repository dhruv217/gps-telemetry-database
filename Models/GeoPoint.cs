using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace gps_telemetry_database_test.Models
{
    public class GeoPoint
    {
        public long Id { get; set; }
        public string Name { get; set; }
        [Required]
        [Column(TypeName = "Decimal (8,6)")]
        public decimal Latitude { get; set; }
        [Required]
        [Column(TypeName = "Decimal (9,6)")]
        public decimal Longitude { get; set; }
        public string Address { get; set; }

    }
}