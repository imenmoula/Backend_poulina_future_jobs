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
    public class PostesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PostesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Postes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Poste>>> GetPostes()
        {
            return await _context.Postes.ToListAsync();
        }

        // GET: api/Postes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Poste>> GetPoste(Guid id)
        {
            var poste = await _context.Postes.FindAsync(id);

            if (poste == null)
            {
                return NotFound();
            }

            return poste;
        }

        // PUT: api/Postes/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPoste(Guid id, Poste poste)
        {
            if (id != poste.IdPoste)
            {
                return BadRequest();
            }

            _context.Entry(poste).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PosteExists(id))
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

        // POST: api/Postes
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Poste>> PostPoste(Poste poste)
        {
            _context.Postes.Add(poste);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPoste", new { id = poste.IdPoste }, poste);
        }

        // DELETE: api/Postes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePoste(Guid id)
        {
            var poste = await _context.Postes.FindAsync(id);
            if (poste == null)
            {
                return NotFound();
            }

            _context.Postes.Remove(poste);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PosteExists(Guid id)
        {
            return _context.Postes.Any(e => e.IdPoste == id);
        }
    }
}
