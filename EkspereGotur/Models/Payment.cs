using System.ComponentModel.DataAnnotations;

namespace EkspereGotur.Models;
public class Payment
{
    public int Id { get; set; }

    public int RequestId { get; set; }
    public ExpertRequest Request { get; set; }

    public int PayerUserId { get; set; }
    public User PayerUser { get; set; }

    public int ReceiverUserId { get; set; }
    public User ReceiverUser { get; set; }

    public decimal Amount { get; set; }
    public string Status { get; set; } // Pending, Released, Refunded
    public DateTime PaidAt { get; set; } = DateTime.Now;
}
