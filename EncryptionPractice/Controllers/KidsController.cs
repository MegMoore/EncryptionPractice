using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EncryptionPractice.Data;
using EncryptionPractice.Models;

namespace EncryptionPractice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KidsController : ControllerBase
    {
        private readonly EncryptionPracticeContext _context;

        public KidsController(EncryptionPracticeContext context)
        {
            _context = context;
        }

        // GET: api/Kids
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Kid>>> GetKid()
        {
            return await _context.Kids.ToListAsync();
        }

        // GET: api/Kids/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Kid>> GetKid(int id)
        {
            var kid = await _context.Kids.FindAsync(id);

            if (kid == null)
            {
                return NotFound();
            }

            return kid;
        }

        // PUT: api/Kids/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutKid(int id, Kid kid)
        {
            if (id != kid.Id)
            {
                return BadRequest();
            }

            _context.Entry(kid).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!KidExists(id))
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

        // POST: api/Kids
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Kid>> PostKid(Kid kid)
        {
            _context.Kids.Add(kid);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetKid", new { id = kid.Id }, kid);
        }

        // DELETE: api/Kids/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteKid(int id)
        {
            var kid = await _context.Kids.FindAsync(id);
            if (kid == null)
            {
                return NotFound();
            }

            _context.Kids.Remove(kid);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool KidExists(int id)
        {
            return _context.Kids.Any(e => e.Id == id);
        }
    }
}
