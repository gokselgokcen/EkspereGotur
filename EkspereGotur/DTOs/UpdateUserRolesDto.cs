// Dtos/UpdateUserRolesDto.cs
namespace EkspereGotur.Dtos
{
    public class UpdateUserRolesDto
    {
        /// <summary>
        /// Yeni atanacak rollerin listesi (ör. ["User","Admin"])
        /// </summary>
        public List<string> Roles { get; set; }
    }
}
