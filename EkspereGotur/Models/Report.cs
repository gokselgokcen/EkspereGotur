using System.ComponentModel.DataAnnotations;

namespace EkspereGotur.Models;

public class Report
{
    public int Id { get; set; }

    public int RequestId { get; set; }
    public ExpertRequest Request { get; set; }

    public int InspectorId { get; set; }
    public User Inspector { get; set; }

    public int AssignmentId { get; set; }
    public Assignment Assignment { get; set; }

    public string? Notes { get; set; }
    public ICollection<ReportPhoto> Photos { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;
}
