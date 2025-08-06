namespace EkspereGotur.Models
{
    public class ReportPhoto
    {
        public int Id { get; set; }

        // Hangi rapora ait
        public int ReportId { get; set; }
        public Report Report { get; set; }

        // Dosya yolu / URL
        public string Url { get; set; }

    }
}
