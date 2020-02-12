using Microsoft.EntityFrameworkCore;

namespace gps_telemetry_database_test.Models
{
    public class TelemetryDBContext : DbContext
    {
        public TelemetryDBContext(DbContextOptions<TelemetryDBContext> options)
            : base(options)
        {
        }
        public DbSet<GeoPoint> GeoPoints { get; set; }
    }
}