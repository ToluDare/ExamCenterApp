using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExamCenterApp.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedExamSession_Viewbag : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Invigilators_Id",
                table: "Exam_Sessions",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Exam_Sessions_Invigilators_Id",
                table: "Exam_Sessions",
                column: "Invigilators_Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Exam_Sessions_AspNetUsers_Invigilators_Id",
                table: "Exam_Sessions",
                column: "Invigilators_Id",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Exam_Sessions_AspNetUsers_Invigilators_Id",
                table: "Exam_Sessions");

            migrationBuilder.DropIndex(
                name: "IX_Exam_Sessions_Invigilators_Id",
                table: "Exam_Sessions");

            migrationBuilder.DropColumn(
                name: "Invigilators_Id",
                table: "Exam_Sessions");
        }
    }
}
