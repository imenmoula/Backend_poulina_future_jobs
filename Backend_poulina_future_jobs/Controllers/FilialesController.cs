using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Backend_poulina_future_jobs.Models;

namespace Backend_poulina_future_jobs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FilialesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public FilialesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Filiales
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Filiale>>> GetFiliales()
        {
            return await _context.Filiales.ToListAsync();
        }

        // GET: api/Filiales/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Filiale>> GetFiliale(Guid id)
        {
            var filiale = await _context.Filiales.FindAsync(id);

            if (filiale == null)
            {
                return NotFound();
            }

            return filiale;
        }

        // PUT: api/Filiales/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFiliale(Guid id, Filiale filiale)
        {
            if (id != filiale.IdFiliale)
            {
                return BadRequest();
            }

            _context.Entry(filiale).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FilialeExists(id))
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

        // POST: api/Filiales
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Filiale>> PostFiliale(Filiale filiale)
        {
            _context.Filiales.Add(filiale);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetFiliale", new { id = filiale.IdFiliale }, filiale);
        }

        // DELETE: api/Filiales/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFiliale(Guid id)
        {
            var filiale = await _context.Filiales.FindAsync(id);
            if (filiale == null)
            {
                return NotFound();
            }

            _context.Filiales.Remove(filiale);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool FilialeExists(Guid id)
        {
            return _context.Filiales.Any(e => e.IdFiliale == id);
        }
    }
}
