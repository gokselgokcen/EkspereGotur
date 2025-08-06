

using System.ComponentModel.DataAnnotations;

namespace EkspereGotur.Models;

public class ExpertRequest
{
    public int Id { get; set; }

    public int UserId { get; set; }
    public User User { get; set; }

    public string? Plate { get; set; }
    public string? Brand { get; set; }
    public string? Model { get; set; }
    public string? City { get; set; }

    public string District { get; set; }

    public string? Address { get; set; }
    public string? Link { get; set; }
    public string? Notes { get; set; }
    public string? Status { get; set; }  // Pending, Assigned, Completed

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public Assignment? Assignment { get; set; }
    public ICollection<Report> Reports { get; set; }
    public ICollection<Payment> Payments { get; set; }
}
