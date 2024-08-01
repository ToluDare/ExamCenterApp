using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExamCenterApp.Migrations
{
    /// <inheritdoc />
    public partial class StudentController : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "first_name",
                table: "Student",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "last_name",
                table: "Student",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "first_name",
                table: "Student");

            migrationBuilder.DropColumn(
                name: "last_name",
                table: "Student");
        }
    }
}
