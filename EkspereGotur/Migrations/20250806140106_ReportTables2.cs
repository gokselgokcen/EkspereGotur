using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EkspereGotur.Migrations
{
    /// <inheritdoc />
    public partial class ReportTables2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Caption",
                table: "ReportPhotos");

            migrationBuilder.RenameColumn(
                name: "UploadedAt",
                table: "Reports",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "Reports",
                newName: "Notes");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Notes",
                table: "Reports",
                newName: "Description");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Reports",
                newName: "UploadedAt");

            migrationBuilder.AddColumn<string>(
                name: "Caption",
                table: "ReportPhotos",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
