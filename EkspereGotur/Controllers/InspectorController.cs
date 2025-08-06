// Controllers/InspectorsController.cs
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
    [Authorize]  // Giriş yapmış herkes isteyebilir
    public class InspectorsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public InspectorsController(AppDbContext context)
            => _context = context;



        [HttpPost("apply")]
        [SwaggerOperation(Summary = "Apply to become an inspector")]
        public async Task<IActionResult> ApplyToBeInspector([FromBody] ApplyInspectorDto dto)
        {
            // 1) Token’dan userId al
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            // 2) Kullanıcıyı ve rollerini çek
            var user = await _context.Users
                .Include(u => u.UserRoles)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                return NotFound("User not found.");

            // 3) DTO’daki bilgileri kullanıcıya ata
            user.TCNo = dto.TCNo;
            user.IBAN = dto.IBAN;
            user.Address = dto.Address;
            user.OnayDurumu = false;    // admin onayı beklemede

            // 4) “Expert” rolünü ata (UserRoles tablosuna)
            var expertRole = await _context.Roles
                .FirstOrDefaultAsync(r => r.RoleName == "Expert");
            if (expertRole == null)
                return StatusCode(500, "Expert role not found.");

            if (!user.UserRoles.Any(ur => ur.RoleId == expertRole.Id))
            {
                user.UserRoles.Add(new UserRole
                {
                    UserId = userId,
                    RoleId = expertRole.Id
                });
            }

            // 5) Değişiklikleri kaydet
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Your application to become an inspector was submitted. Awaiting admin approval."
            });
        }

        [HttpGet("inspector-status")]
        [SwaggerOperation(Summary = "Returns whether the current user is approved as an inspector")]
        public async Task<IActionResult> GetInspectorStatus()
        {
            // Token’dan userId al
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            // DB'den sadece OnayDurumu alanını çek
            var status = await _context.Users
                .Where(u => u.Id == userId)
                .Select(u => u.OnayDurumu)      // bool? olabilir
                .FirstOrDefaultAsync();

            if (status == null)
            {
                // Henüz başvuru yok
                return Ok(new { isInspectorApplied = false, isApproved = false });
            }

            // status == false → başvuru yapıldı ama onay bekleniyor
            // status == true  → onaylandı
            return Ok(new
            {
                isInspectorApplied = true,
                isApproved = status.Value
            });
        }

        [HttpPost("apply-expertRequest")]
        [Authorize(Roles = "Expert")]
        [SwaggerOperation(Summary = "Apply to an expert request")]
        public async Task<IActionResult> Apply([FromBody] CreateApplicationDto dto)
        {
            var inspectorId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            // 1) İlgili ilanın varlığını ve durumunu kontrol et
            var req = await _context.ExpertRequests
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Id == dto.RequestId && r.Status == "Pending");
            if (req == null)
                return BadRequest("Request not found or not open for applications.");

            // 2) Kendi ilanına başvurmayı engelle
            if (req.UserId == inspectorId)
                return BadRequest("You cannot apply to your own request.");

            // 3) Daha önce başvurduysa tekrar etme
            var already = await _context.Assignments
                .AnyAsync(a => a.RequestId == dto.RequestId && a.InspectorId == inspectorId);
            if (already)
                return BadRequest("You have already applied to this request.");

            // 4) Yeni Assignment kaydı oluştur (status = Pending)
            var assignment = new Assignment
            {
                RequestId = dto.RequestId,
                InspectorId = inspectorId,
                AssignedAt = DateTime.UtcNow,
                Status = "Pending"
            };

            _context.Assignments.Add(assignment);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Application submitted successfully",
                assignmentId = assignment.Id
            });
        }
        [AllowAnonymous]
        [HttpGet("open-requests")]
        [SwaggerOperation(Summary = "Get all open expert requests")]
        public async Task<IActionResult> GetOpenExpertRequests()
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
                    r.CreatedAt
                })
                .ToListAsync();

            return Ok(list);
        }
        [Authorize(Roles = "Expert")]
        [HttpGet("my-applications")]
        [SwaggerOperation(Summary = "Get all applications made by the current inspector")]
        public async Task<IActionResult> GetMyApplications()
        {
            var inspectorId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            var apps = await _context.Assignments
                .AsNoTracking()
                .Include(a => a.Request)
                .Where(a => a.InspectorId == inspectorId)
                .Select(a => new
                {
                    a.Id,
                    a.RequestId,
                    Plate = a.Request.Plate,
                    Brand = a.Request.Brand,
                    Model = a.Request.Model,
                    Status = a.Status,
                    Link = a.Request.Link,
                    a.AssignedAt
                })
                .ToListAsync();

            return Ok(apps);
        }
    }
}
