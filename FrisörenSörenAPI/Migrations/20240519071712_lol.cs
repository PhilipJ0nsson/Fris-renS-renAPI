using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FrisörenSörenAPI.Migrations
{
    /// <inheritdoc />
    public partial class lol : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CompanyId",
                table: "ChangeLogs",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CustomerId",
                table: "ChangeLogs",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "ChangeLogs");

            migrationBuilder.DropColumn(
                name: "CustomerId",
                table: "ChangeLogs");
        }
    }
}
