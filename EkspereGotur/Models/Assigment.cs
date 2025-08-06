using System.ComponentModel.DataAnnotations;

namespace EkspereGotur.Models;

public class Assignment
{
    public int Id { get; set; }

    public int RequestId { get; set; }
    public ExpertRequest Request { get; set; }

    public int InspectorId { get; set; }
    public User Inspector { get; set; }

    public string Status { get; set; } = "Pending";

    public Report? Report { get; set; }

    public DateTime AssignedAt { get; set; } = DateTime.Now;
}
