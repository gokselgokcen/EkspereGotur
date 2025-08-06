using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EkspereGotur.Migrations
{
    /// <inheritdoc />
    public partial class requestUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "District",
                table: "ExpertRequests",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "District",
                table: "ExpertRequests");
        }
    }
}
