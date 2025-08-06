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
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly AppDbContext _context;
        public AdminController(AppDbContext context) => _context = context;

        /// <summary>
        /// Beklemedeki tüm “inspector” başvurularını listeler.
        /// </summary>
        [HttpGet("inspector-applications")]
        [SwaggerOperation(Summary = "List pending inspector applications")]
        public async Task<IActionResult> GetInspectorApplications()
        {
            var list = await _context.Users
                .Include(u => u.UserRoles).ThenInclude(ur => ur.Role)
                .Where(u => u.UserRoles.Any(ur => ur.Role.RoleName == "Expert")
                         && u.OnayDurumu == false)
                .Select(u => new InspectorApplicationDto
                {
                    UserId = u.Id,
                    Email = u.Email,
                    Name = u.Name,
                    Surname = u.Surname,
                    GSM = u.GSM,
                    TCNo = u.TCNo ?? "",
                    IBAN = u.IBAN ?? "",
                    Address = u.Address ?? "",
                    OnayDurumu = u.OnayDurumu
                })
                .ToListAsync();

            return Ok(list);
        }

        /// <summary>
        /// Bir “inspector” başvurusunu onaylar.
        /// </summary>
        [HttpPost("inspector-applications/{userId}/approve")]
        [SwaggerOperation(Summary = "Approve an inspector application")]
        public async Task<IActionResult> ApproveInspector(int userId)
        {
            var user = await _context.Users
                .Include(u => u.UserRoles)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                return NotFound("User not found.");

            // Başvuru yapılmış mı?
            if (user.OnayDurumu != false)
                return BadRequest("No pending application for this user.");

            // Onayla
            user.OnayDurumu = true;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Inspector application approved." });
        }

        /// <summary>
        /// Bir kullanıcıya yeni roller atar (mevcut roller silinir).
        /// </summary>
        [HttpPut("users/{userId}/roles")]
        [SwaggerOperation(Summary = "Update roles for a user")]
        public async Task<IActionResult> UpdateUserRoles(int userId, [FromBody] UpdateUserRolesDto dto)
        {
            var user = await _context.Users
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                return NotFound("User not found.");

            // Tüm mevcut rolleri temizle
            _context.UserRoles.RemoveRange(user.UserRoles);

            // Yeni rolleri ekle
            var roles = await _context.Roles
                .Where(r => dto.Roles.Contains(r.RoleName))
                .ToListAsync();

            foreach (var role in roles)
            {
                user.UserRoles.Add(new UserRole
                {
                    UserId = userId,
                    RoleId = role.Id
                });
            }

            await _context.SaveChangesAsync();
            return Ok(new { message = "User roles updated." });
        }
    }
}
