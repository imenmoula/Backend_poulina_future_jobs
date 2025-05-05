using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Backend_poulina_future_jobs.Models;
using Microsoft.AspNetCore.Authorization;

namespace Backend_poulina_future_jobs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DiplomesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public DiplomesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Diplomes
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<Diplome>>> GetDiplomes()
        {
            return await _context.Diplomes.ToListAsync();
        }

        // GET: api/Diplomes/5
        [HttpGet("{id}")]
        [AllowAnonymous]

        public async Task<ActionResult<Diplome>> GetDiplome(Guid id)
        {
            var diplome = await _context.Diplomes.FindAsync(id);

            if (diplome == null)
            {
                return NotFound();
            }

            return diplome;
        }

        // PUT: api/Diplomes/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [AllowAnonymous]

        public async Task<IActionResult> PutDiplome(Guid id, Diplome diplome)
        {
            if (id != diplome.IdDiplome)
            {
                return BadRequest();
            }

            _context.Entry(diplome).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DiplomeExists(id))
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

        // POST: api/Diplomes
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [AllowAnonymous]

        public async Task<ActionResult<Diplome>> PostDiplome(Diplome diplome)
        {
            _context.Diplomes.Add(diplome);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetDiplome", new { id = diplome.IdDiplome }, diplome);
        }

        // DELETE: api/Diplomes/5
        [HttpDelete("{id}")]
        [AllowAnonymous]

        public async Task<IActionResult> DeleteDiplome(Guid id)
        {
            var diplome = await _context.Diplomes.FindAsync(id);
            if (diplome == null)
            {
                return NotFound();
            }

            _context.Diplomes.Remove(diplome);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool DiplomeExists(Guid id)
        {
            return _context.Diplomes.Any(e => e.IdDiplome == id);
        }
    }
}
