using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Gruppe6Oppgave2.Migrations
{
    /// <inheritdoc />
    public partial class object_geometry_type : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "GeometryType",
                table: "HindranceObjects",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GeometryType",
                table: "HindranceObjects");
        }
    }
}
