using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Backend_poulina_future_jobs.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace Backend_poulina_future_jobs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public RolesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Roles
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<IdentityRole<Guid>>>> GetRoles()
        {
            return await _context.Roles.ToListAsync();
        }
        // GET: api/Roles/5
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<IdentityRole<Guid>>> GetAppRole(Guid id)
        {
            var role = await _context.Roles.FindAsync(id);

            if (role == null)
            {
                return NotFound();
            }

            return role;
        }


        // PUT: api/Roles/5
        [HttpPut("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> PutAppRole(Guid id, AppRole updatedRole)
        {
            var existingRole = await _context.Roles.FindAsync(id);
            if (existingRole == null)
            {
                return NotFound();
            }

            existingRole.Name = updatedRole.Name;
            existingRole.NormalizedName = updatedRole.Name.ToUpper();

            // Assurer que ConcurrencyStamp reste null après modification
            existingRole.ConcurrencyStamp = null;

            _context.Roles.Update(existingRole);
            await _context.SaveChangesAsync();

            return NoContent();
        }



        // POST: api/Roles
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<AppRole>> PostAppRole(AppRole appRole)
        {
            if (string.IsNullOrEmpty(appRole.Name))
            {
                return BadRequest("Le nom du rôle est requis.");
            }

            // Normaliser le nom du rôle
            appRole.NormalizedName = appRole.Name.ToUpper();

            // Forcer ConcurrencyStamp à null
            appRole.ConcurrencyStamp = null;

            _context.Roles.Add(appRole);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAppRole), new { id = appRole.Id }, appRole);
        }

        // DELETE: api/Roles/5
        [HttpDelete("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> DeleteAppRole(Guid id)
        {
            var appRole = await _context.Roles.FindAsync(id);
            if (appRole == null)
            {
                return NotFound();
            }

            _context.Roles.Remove(appRole);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool AppRoleExists(Guid id)
        {
            return _context.Roles.Any(e => e.Id == id);
        }
    }
}
