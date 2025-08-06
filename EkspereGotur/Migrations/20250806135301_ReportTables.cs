using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EkspereGotur.Migrations
{
    /// <inheritdoc />
    public partial class ReportTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AssignmentId",
                table: "Reports",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "ReportPhotos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReportId = table.Column<int>(type: "int", nullable: false),
                    Url = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Caption = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportPhotos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReportPhotos_Reports_ReportId",
                        column: x => x.ReportId,
                        principalTable: "Reports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Reports_AssignmentId",
                table: "Reports",
                column: "AssignmentId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ReportPhotos_ReportId",
                table: "ReportPhotos",
                column: "ReportId");

            migrationBuilder.AddForeignKey(
                name: "FK_Reports_Assignments_AssignmentId",
                table: "Reports",
                column: "AssignmentId",
                principalTable: "Assignments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reports_Assignments_AssignmentId",
                table: "Reports");

            migrationBuilder.DropTable(
                name: "ReportPhotos");

            migrationBuilder.DropIndex(
                name: "IX_Reports_AssignmentId",
                table: "Reports");

            migrationBuilder.DropColumn(
                name: "AssignmentId",
                table: "Reports");
        }
    }
}
