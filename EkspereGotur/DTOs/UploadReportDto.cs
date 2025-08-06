// Dtos/UploadReportDto.cs
using Microsoft.AspNetCore.Http;

namespace EkspereGotur.Dtos
{
    public class UploadReportDto
    {
        public int AssignmentId { get; set; }
        public string? Notes    { get; set; }
        public IFormFileCollection Files { get; set; }
    }
}
