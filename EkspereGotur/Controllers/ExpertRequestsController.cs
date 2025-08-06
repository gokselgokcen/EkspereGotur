using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using EkspereGotur.Data;
using EkspereGotur.Dtos;
using EkspereGotur.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;

namespace EkspereGotur.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ExpertRequestsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ExpertRequestsController(AppDbContext context)
        {
            _context = context;
        }

        
        [HttpGet("my")]
        [SwaggerOperation(Summary = "Kullanıcının kendi ekspertiz isteklerini getirir")]
        public async Task<IActionResult> GetMyRequests()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            var list = await _context.ExpertRequests
                .Where(er => er.UserId == userId)
                .Select(er => new
                {
                    er.Id,
                    er.Plate,
                    er.Brand,
                    er.Model,
                    er.Status,
                    er.CreatedAt
                })
                .ToListAsync();

            return Ok(list);
        }

        /// <summary>
        /// Belirli bir ekspertiz isteğinin detayını getirir (sadece sahibi görebilir).
        /// </summary>
        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Belirli bir ekspertiz ilanını getirir")]
        public async Task<IActionResult> GetById(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            var er = await _context.ExpertRequests
                .Include(r => r.Assignment)
                .FirstOrDefaultAsync(r => r.Id == id && r.UserId == userId);

            if (er == null)
                return NotFound();

            return Ok(new
            {
                er.Id,
                er.Plate,
                er.Brand,
                er.Model,
                er.City,
                er.District,
                er.Address,
                er.Link,
                er.Notes,
                er.Status,
                er.CreatedAt,
                AssignedInspectorId = er.Assignment?.InspectorId
            });
        }

        // ---- WRITE ----

        /// <summary>
        /// Yeni bir ekspertiz ilanı açar.
        /// </summary>
        [HttpPost]
        [SwaggerOperation(Summary = "İlan açma")]
        public async Task<IActionResult> Create([FromBody] CreateExpertRequestDto request)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            var newRequest = new ExpertRequest
            {
                UserId    = userId,
                Plate     = request.Plate,
                Brand     = request.Brand,
                Model     = request.Model,
                City      = request.City,
                District  = request.District,
                Address   = request.Address,
                Link      = request.Link,
                Notes     = request.Notes,
                CreatedAt = DateTime.UtcNow,
                Status    = "Pending"
            };

            _context.ExpertRequests.Add(newRequest);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message   = "Expert request created successfully",
                requestId = newRequest.Id
            });
        }

        /// <summary>
        /// Onaylanmamış (Pending) bir ilanı siler (sadece sahibi ve Pending durumunda).
        /// </summary>
        [HttpDelete("{id}")]
        [SwaggerOperation(Summary = "Sadece Pending ilanları siler")]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            var er = await _context.ExpertRequests
                .FirstOrDefaultAsync(r => r.Id == id && r.UserId == userId);

            if (er == null)
                return NotFound();

            if (er.Status != "Pending")
                return BadRequest("Only pending requests can be deleted.");

            _context.ExpertRequests.Remove(er);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
