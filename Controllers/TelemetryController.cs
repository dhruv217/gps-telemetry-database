using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using gps_telemetry_database_test.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace gps_telemetry_database_test.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TelemetryController : ControllerBase
    {
        private Random random = new Random();
        private double THREE_PI = Math.PI * 3;
        private double TWO_PI = Math.PI * 2;

        private static Coordinate Bhopal = new Coordinate
        {
            Latitude = 23.2599,
            Longitude = 77.4126,
        };

        private readonly ILogger<TelemetryController> _logger;
        private readonly TelemetryDBContext _context;
        private readonly IConfiguration _config;


        public TelemetryController(ILogger<TelemetryController> logger, TelemetryDBContext context, IConfiguration config)
        {
            _context = context;
            _logger = logger;
            _config = config;
        }

        [HttpGet("generaterandom/{count}")]
        public async Task<ActionResult> GenerateRamdomGeoPoints(int count)
        {
            for (int i = 0; i < count; i++)
            {
                var randCoord = pointInCircle(Bhopal, 300000);
                _context.GeoPoints.Add(new GeoPoint
                {
                    Name = String.Format("GeoPoint-{0}", random.Next()),
                    Latitude = Math.Round(Convert.ToDecimal(randCoord.Latitude), 6),
                    Longitude = Math.Round(Convert.ToDecimal(randCoord.Longitude), 6),
                    Address = "Lorem ipsum dolor sit amet."
                });
                if (i > 0 && i % 100 == 0)
                {
                    await _context.SaveChangesAsync();
                }
            }
            return Ok();
        }

        [HttpPost("filternearest")]
        public ActionResult filterNearest(NearestGeoPointForm coord) // Coordinate local
        {
            coord.distance = coord.distance / 1000;
            List<NearestFilteredGeoPoint> geoPoints = new List<NearestFilteredGeoPoint>();
            using (var db = new SqlConnection(_config.GetConnectionString("TelemetryDatabase")))
            {
                var sqlString = "SELECT Top 20 * " +
                                "FROM " +
                                    "(SELECT "+
                                        "Id, Name, Latitude, Longitude, Address, "+
                                        "(acos(cos(radians(@latitude)) * cos(radians(Latitude)) * "+
                                        "cos(radians(Longitude) - radians(@longitude)) + sin(radians(@latitude)) * "+
                                        "sin(radians(Latitude))) * 6367) AS Distance "+
                                    "FROM GeoPoints) p "+
                                "WHERE "+
                                    "p.Distance < @distance "+
                                "ORDER BY "+
                                    "p.Distance";
                geoPoints = db.Query<NearestFilteredGeoPoint>(sqlString, coord).ToList();
                return Ok(geoPoints);
            }
        }

        public Coordinate pointAtDistance(Coordinate inputCoords, double distance)
        {
            var result = new Coordinate();

            var coords = toRadians(inputCoords);

            var sinLat = Math.Sin(coords.Latitude);

            var cosLat = Math.Cos(coords.Latitude);

            // go fixed distance in random direction
            var bearing = random.NextDouble() * TWO_PI;

            var theta = distance / 6371000; // EARTH_RADIUS in meters
            var sinBearing = Math.Sin(bearing);

            var cosBearing = Math.Cos(bearing);
            var sinTheta = Math.Sin(theta);

            var cosTheta = Math.Cos(theta);


            result.Latitude = Math.Asin(sinLat * cosTheta + cosLat * sinTheta * cosBearing);
            result.Longitude = coords.Longitude +
                Math.Atan2(sinBearing * sinTheta * cosLat, cosTheta - sinLat * Math.Sin(result.Latitude)
            );
            // normalize -PI -> +PI radians 
            result.Longitude = ((result.Longitude + THREE_PI) % TWO_PI) - Math.PI;
            return toDegrees(result);
        }

       

        public Coordinate pointInCircle(Coordinate coord, double distance)
        {
            var rnd = random.NextDouble();
            // use square root of random number to avoid high density at the center
            var randomDist = Math.Sqrt(rnd) * distance;

            return pointAtDistance(coord, randomDist);
        }

        private double toRadians(double angle)
        {
            return Math.PI * angle / 180.0;
        }

        private Coordinate toRadians(Coordinate coord)
        {
            return new Coordinate
            {
                Latitude = toRadians(coord.Latitude),
                Longitude = toRadians(coord.Longitude)
            };
        }

        private double toDegrees(double angle)
        {
            return angle * (180.0 / Math.PI);
        }

        private Coordinate toDegrees(Coordinate coord)
        {
            return new Coordinate
            {
                Latitude = toDegrees(coord.Latitude),
                Longitude = toDegrees(coord.Longitude)
            };
        }
    }

    public class NearestGeoPointForm {
        public float latitude {get; set;}
        public float longitude { get; set; }
        public float distance { get; set; }
    }

    public class NearestFilteredGeoPoint {
        public long Id { get; set; }
        public string Name { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public string Address { get; set; }
        public decimal Distance { get; set; }
    }
}
