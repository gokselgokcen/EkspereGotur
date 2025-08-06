// Dtos/InspectorApplicationDto.cs
namespace EkspereGotur.Dtos
{
    public class InspectorApplicationDto
    {
        public int    UserId             { get; set; }
        public string Email              { get; set; }
        public string Name           { get; set; }
        public string Surname           { get; set; }
        public string GSM                { get; set; }
        public string TCNo { get; set; }
        public string IBAN               { get; set; }
        public string Address            { get; set; }
        public bool?  OnayDurumu        { get; set; }
    }
}
