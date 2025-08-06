// Dtos/ReportDetailDto.cs
namespace EkspereGotur.Dtos
{
    public class ReportDetailDto
    {
        public int Id            { get; set; }
        public int AssignmentId  { get; set; }
        public string? Notes     { get; set; }
        public DateTime CreatedAt{ get; set; }
        public List<string> PhotoUrls { get; set; }
    }
}
