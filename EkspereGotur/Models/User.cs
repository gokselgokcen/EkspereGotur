using System.ComponentModel.DataAnnotations;

namespace EkspereGotur.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        public string? Name { get; set; }
        public string? Surname { get; set; }
        public string? GSM { get; set; }
        public string? IBAN { get; set; }

        public string? TCNo { get; set; }
        public string? Address { get; set; }

        public bool? OnayDurumu { get; set; } = null;  // admin onayı durumu

        public ICollection<UserRole> UserRoles { get; set; }
        public ICollection<ExpertRequest> Requests { get; set; }  // açtığı ilanlar
        public ICollection<Assignment> Assignments { get; set; }  // atandığı görevler
        public ICollection<Report> Reports { get; set; }          // yüklediği raporlar
        public ICollection<Payment> PaymentsMade { get; set; }    // yaptığı ödemeler
        public ICollection<Payment> PaymentsReceived { get; set; } // aldığı ödemeler
    }

}
