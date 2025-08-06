using System;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using EkspereGotur.Data;
using EkspereGotur.Dtos;
using EkspereGotur.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;

namespace EkspereGotur.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ReportsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public ReportsController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        /// <summary>
        /// Expert → Bir atamaya (assignment) ait tek bir rapor oluşturur, 
        ///      içine birden çok fotoğraf yükler.
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Expert")]
        [SwaggerOperation(Summary = "Create a report with multiple photos")]
        public async Task<IActionResult> Create([FromForm] UploadReportDto dto)
        {
            var inspectorId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            // 1) Atamanın gerçekten bu görevliye ait olduğunu doğrula
            var assignment = await _context.Assignments
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Id == dto.AssignmentId && a.InspectorId == inspectorId);
            if (assignment == null)
                return BadRequest("Invalid assignment or you are not the assigned inspector.");

            // 2) Rapor kaydını oluştur (henüz fotoğraf eklemedik)
            var report = new Report
            {
                AssignmentId = dto.AssignmentId,
                RequestId    = assignment.RequestId,
                InspectorId = assignment.InspectorId,
                Notes = dto.Notes,
                CreatedAt = DateTime.UtcNow
            };
            _context.Reports.Add(report);
            await _context.SaveChangesAsync(); // report.Id’yi almak için

            // 3) Dosyaları kaydet ve ReportPhoto kayıtları oluştur
            //    (tek döngü, tek uploadFolder tanımı)
            var uploadFolder = Path.Combine(_env.WebRootPath, "uploads", "reports");
            if (!Directory.Exists(uploadFolder))
                Directory.CreateDirectory(uploadFolder);

            if (dto.Files != null && dto.Files.Count > 0)
            {
                foreach (var file in dto.Files)
                {
                    if (file.Length == 0)
                        continue;

                    var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
                    var filePath = Path.Combine(uploadFolder, fileName);

                    // Fiziksel dosyayı kaydet
                    using var fs = System.IO.File.Create(filePath);
                    await file.CopyToAsync(fs);

                    // DB kaydını oluştur
                    _context.ReportPhotos.Add(new ReportPhoto
                    {
                        ReportId = report.Id,
                        Url = $"/uploads/reports/{fileName}"
                    });
                }

                await _context.SaveChangesAsync();
            }

            return Ok(new { reportId = report.Id });
        }

        /// <summary>
        /// Expert ve User → Bir atamanın raporunu ve fotoğraflarını getirir.
        /// </summary>
        [HttpGet("assignment/{assignmentId}")]
        [SwaggerOperation(Summary = "Get report details (and photos) by assignment")]
        public async Task<IActionResult> GetByAssignment(int assignmentId)
        {
            // Dilersen burada assignment.UserId == currentUser kontrolü ekleyebilirsin
            var report = await _context.Reports
                .Include(r => r.Photos)
                .FirstOrDefaultAsync(r => r.AssignmentId == assignmentId);

            if (report == null)
                return NotFound();

            var dto = new ReportDetailDto
            {
                Id = report.Id,
                AssignmentId = report.AssignmentId,
                Notes = report.Notes,
                CreatedAt = report.CreatedAt,
                PhotoUrls = report.Photos.Select(p => p.Url).ToList()
            };

            return Ok(dto);
        }

        [Authorize(Roles = "Expert")]
        [HttpGet("{reportId}")]
        [SwaggerOperation(Summary = "Get report details by report id")]
        public async Task<IActionResult> Get(int reportId)
        {
            var report = await _context.Reports
                .Include(r => r.Photos)
                .FirstOrDefaultAsync(r => r.Id == reportId);

            if (report == null)
                return NotFound();

            var dto = new ReportDetailDto
            {
                Id = report.Id,
                AssignmentId = report.AssignmentId,
                Notes = report.Notes,
                CreatedAt = report.CreatedAt,
                PhotoUrls = report.Photos.Select(p => p.Url).ToList()
            };

            return Ok(dto);
        }
    }
}
