using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DentalClinic.Migrations
{
    /// <inheritdoc />
    public partial class editPatientHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Patients_PatientHistoryId",
                table: "Patients");

            migrationBuilder.CreateIndex(
                name: "IX_Patients_PatientHistoryId",
                table: "Patients",
                column: "PatientHistoryId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Patients_PatientHistoryId",
                table: "Patients");

            migrationBuilder.CreateIndex(
                name: "IX_Patients_PatientHistoryId",
                table: "Patients",
                column: "PatientHistoryId");
        }
    }
}
