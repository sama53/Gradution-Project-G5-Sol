using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Gradution_Project_G5.DAL.Migrations
{
    /// <inheritdoc />
    public partial class init1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Grades_TraineeId",
                table: "Grades");

            migrationBuilder.CreateIndex(
                name: "IX_Grades_TraineeId_SessionId",
                table: "Grades",
                columns: new[] { "TraineeId", "SessionId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Grades_TraineeId_SessionId",
                table: "Grades");

            migrationBuilder.CreateIndex(
                name: "IX_Grades_TraineeId",
                table: "Grades",
                column: "TraineeId");
        }
    }
}
