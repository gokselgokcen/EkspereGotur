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
    public class ApplicationsController : ControllerBase
    {
        private readonly AppDbContext _context;
        public ApplicationsController(AppDbContext context) => _context = context;

        // ---- READ ----

        /// <summary>
        /// Tüm açık (Pending) ekspertiz isteklerini getirir.
        /// </summary>
        [AllowAnonymous]
        [HttpGet("open")]
        [SwaggerOperation(Summary = "Get all open expert requests")]
        public async Task<IActionResult> GetOpenRequests()
        {
            var list = await _context.ExpertRequests
                .Where(r => r.Status == "Pending")
                .Select(r => new
                {
                    r.Id,
                    r.Plate,
                    r.Brand,
                    r.Model,
                    r.City,
                    r.District,
                    r.Link,
                    r.CreatedAt
                })
                .ToListAsync();

            return Ok(list);
        }

        /// <summary>
        /// Inspector → Kendi yaptığı başvuruları listeler.
        /// </summary>
        [Authorize(Roles = "Expert")]
        [HttpGet("my")]
        [SwaggerOperation(Summary = "Get my applications")]
        public async Task<IActionResult> GetMyApplications()
        {
            var inspectorId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            var list = await _context.Assignments
                .Include(a => a.Request)
                .Where(a => a.InspectorId == inspectorId)
                .Select(a => new
                {
                    a.Id,
                    a.RequestId,
                    a.Request.Plate,
                    a.Request.Brand,
                    a.Request.Model,
                    a.AssignedAt
                })
                .ToListAsync();

            return Ok(list);
        }

        

       [Authorize(Roles = "User")]
        [HttpPost("{id}/accept")]
        [SwaggerOperation(Summary = "user kabul etme endpointi")]
        public async Task<IActionResult> Accept(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            // İlgili atamayı ve ilanı yükle
            var assignment = await _context.Assignments
                .Include(a => a.Request)
                .FirstOrDefaultAsync(a => a.Id == id && a.Request.UserId == userId);

            if (assignment == null)
                return NotFound();

            if (assignment.Status != "Pending")
                return BadRequest("Only pending applications can be accepted.");

            // 1) Assignment.Status'ı güncelle
            assignment.Status = "Accepted";

            // 2) İlanın (ExpertRequest) Status'ını güncelle
            assignment.Request.Status = "Assigned";

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Application accepted; inspector is now assigned."
            });
        }

        
    }
}
