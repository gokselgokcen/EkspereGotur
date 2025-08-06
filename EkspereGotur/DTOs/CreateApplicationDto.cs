namespace EkspereGotur.Dtos
{
    public class CreateApplicationDto
    {
        /// <summary>
        /// Başvurulacak ExpertRequest’in Id’si
        /// </summary>
        public int RequestId { get; set; }

        /// <summary>
        /// (Opsiyonel) Başvuru ile birlikte bırakılacak not
        /// </summary>
        public string? Note { get; set; }
    }
}
