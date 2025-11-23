using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Gruppe6Oppgave2.Migrations
{
    /// <inheritdoc />
    public partial class siste_forandringer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "HindranceObjects");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "HindranceObjects");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Latitude",
                table: "HindranceObjects",
                type: "double",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Longitude",
                table: "HindranceObjects",
                type: "double",
                nullable: false,
                defaultValue: 0.0);
        }
    }
}
