using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using gps_telemetry_database_test.Models;

namespace gps_telemetry_database_test.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GeoPointsController : ControllerBase
    {
        private readonly TelemetryDBContext _context;

        public GeoPointsController(TelemetryDBContext context)
        {
            _context = context;
        }

        // GET: api/GeoPoints
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GeoPoint>>> GetGeoPoints()
        {
            return await _context.GeoPoints.ToListAsync();
        }

        // GET: api/GeoPoints/5
        [HttpGet("{id}")]
        public async Task<ActionResult<GeoPoint>> GetGeoPoint(long id)
        {
            var geoPoint = await _context.GeoPoints.FindAsync(id);

            if (geoPoint == null)
            {
                return NotFound();
            }

            return geoPoint;
        }

        // PUT: api/GeoPoints/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutGeoPoint(long id, GeoPoint geoPoint)
        {
            if (id != geoPoint.Id)
            {
                return BadRequest();
            }

            _context.Entry(geoPoint).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GeoPointExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // India Bounding gps locations {'sw': {'lat': 6.5546079, 'lon': 68.1113787}, 'ne': {'lat': 35.6745457, 'lon': 97.395561}} (68.1766451354, 7.96553477623, 97.4025614766, 35.4940095078)
        // [[[73.9112365246,20.884174916],[82.3267638683,20.884174916],[82.3267638683,26.6127981822],[73.9112365246,26.6127981822],[73.9112365246,20.884174916]]]
        
        
        
        
        // POST: api/GeoPoints
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<GeoPoint>> PostGeoPoint(GeoPoint geoPoint)
        {
            _context.GeoPoints.Add(geoPoint);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetGeoPoint", new { id = geoPoint.Id }, geoPoint);
        }

        // DELETE: api/GeoPoints/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<GeoPoint>> DeleteGeoPoint(long id)
        {
            var geoPoint = await _context.GeoPoints.FindAsync(id);
            if (geoPoint == null)
            {
                return NotFound();
            }

            _context.GeoPoints.Remove(geoPoint);
            await _context.SaveChangesAsync();

            return geoPoint;
        }

        private bool GeoPointExists(long id)
        {
            return _context.GeoPoints.Any(e => e.Id == id);
        }
    }
}
