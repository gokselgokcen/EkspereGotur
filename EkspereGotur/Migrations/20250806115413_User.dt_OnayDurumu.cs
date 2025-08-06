using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EkspereGotur.Migrations
{
    /// <inheritdoc />
    public partial class Userdt_OnayDurumu : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "IBAN",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "OnayDurumu",
                table: "Users",
                type: "bit",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IBAN",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "OnayDurumu",
                table: "Users");
        }
    }
}
