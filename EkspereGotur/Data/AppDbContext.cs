using EkspereGotur.Models;
using Microsoft.EntityFrameworkCore;

namespace EkspereGotur.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<User>           Users          { get; set; }
        public DbSet<Role>           Roles          { get; set; }
        public DbSet<UserRole>       UserRoles      { get; set; }
        public DbSet<ExpertRequest>  ExpertRequests { get; set; }
        public DbSet<Assignment>     Assignments    { get; set; }
        public DbSet<Report>         Reports        { get; set; }
        public DbSet<ReportPhoto>    ReportPhotos   { get; set; }
        public DbSet<Payment>        Payments       { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // User–Role many-to-many
            modelBuilder.Entity<UserRole>()
                .HasKey(ur => new { ur.UserId, ur.RoleId });
            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.UserId);
            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(ur => ur.RoleId);

            // Payments – no cascade on either side
            modelBuilder.Entity<Payment>()
                .HasOne(p => p.PayerUser)
                .WithMany(u => u.PaymentsMade)
                .HasForeignKey(p => p.PayerUserId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Payment>()
                .HasOne(p => p.ReceiverUser)
                .WithMany(u => u.PaymentsReceived)
                .HasForeignKey(p => p.ReceiverUserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Assignment ↔ ExpertRequest (1:1)
            modelBuilder.Entity<Assignment>()
                .HasOne(a => a.Request)
                .WithOne(r => r.Assignment)
                .HasForeignKey<Assignment>(a => a.RequestId)
                .OnDelete(DeleteBehavior.Cascade);

            // Assignment ↔ Inspector(User) (N:1) – no cascade on delete
            modelBuilder.Entity<Assignment>()
                .HasOne(a => a.Inspector)
                .WithMany(u => u.Assignments)
                .HasForeignKey(a => a.InspectorId)
                .OnDelete(DeleteBehavior.Restrict);

            // Report ↔ ExpertRequest (N:1)
            modelBuilder.Entity<Report>()
                .HasOne(r => r.Request)
                .WithMany(er => er.Reports)
                .HasForeignKey(r => r.RequestId)
                .OnDelete(DeleteBehavior.Cascade);

            // Report ↔ Inspector(User) (N:1) – no cascade on delete
            modelBuilder.Entity<Report>()
                .HasOne(r => r.Inspector)
                .WithMany(u => u.Reports)
                .HasForeignKey(r => r.InspectorId)
                .OnDelete(DeleteBehavior.Restrict);

            // Report ↔ Assignment (1:1)
            modelBuilder.Entity<Report>()
                .HasOne(r => r.Assignment)
                .WithOne(a => a.Report)
                .HasForeignKey<Report>(r => r.AssignmentId)
                .OnDelete(DeleteBehavior.Restrict);

            // ReportPhoto ↔ Report (N:1)
            modelBuilder.Entity<ReportPhoto>()
                .HasOne(p => p.Report)
                .WithMany(r => r.Photos)
                .HasForeignKey(p => p.ReportId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
