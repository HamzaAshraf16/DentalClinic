using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DentalClinic.Migrations
{
    /// <inheritdoc />
    public partial class age : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Age",
                table: "Patients",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Doctors",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Branchs",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Doctors_UserId",
                table: "Doctors",
                column: "UserId",
                unique: true,
                filter: "[UserId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Branchs_UserId",
                table: "Branchs",
                column: "UserId",
                unique: true,
                filter: "[UserId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Branchs_AspNetUsers_UserId",
                table: "Branchs",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Doctors_AspNetUsers_UserId",
                table: "Doctors",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Branchs_AspNetUsers_UserId",
                table: "Branchs");

            migrationBuilder.DropForeignKey(
                name: "FK_Doctors_AspNetUsers_UserId",
                table: "Doctors");

            migrationBuilder.DropIndex(
                name: "IX_Doctors_UserId",
                table: "Doctors");

            migrationBuilder.DropIndex(
                name: "IX_Branchs_UserId",
                table: "Branchs");

            migrationBuilder.DropColumn(
                name: "Age",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Doctors");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Branchs");
        }
    }
}
