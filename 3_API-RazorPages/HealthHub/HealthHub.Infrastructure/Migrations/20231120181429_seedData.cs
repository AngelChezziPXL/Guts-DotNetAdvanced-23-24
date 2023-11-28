using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthHub.Infrastructure.Migrations
{
    public partial class seedData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Appointments",
                columns: new[] { "Id", "AppointmentDate", "DoctorId", "PatientNationalNumber", "Reason" },
                values: new object[] { 9999, new DateTime(2023, 11, 20, 0, 0, 0, 0, DateTimeKind.Local), 1, "111111", "reason" });

            migrationBuilder.InsertData(
                table: "Specialties",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Cardiology" },
                    { 2, "Dermatoloty" },
                    { 3, "Orthopedics" },
                    { 4, "Neurology" },
                    { 5, "Pediatrics" }
                });

            migrationBuilder.InsertData(
                table: "Doctors",
                columns: new[] { "Id", "Email", "FirstName", "LastName", "Phone", "SpecialtyId" },
                values: new object[,]
                {
                    { 1, "dr.sophie.vandamme@example.com", "Sophie", "Van Damme", "+32 479 12 34 56", 1 },
                    { 2, "dr.Thomas.Devox@example.com", "Thomas", "De Vos", "+32 473 98 76 54", 1 },
                    { 3, "dr.Marie.Dubois@example.com", "Marie", "Dubois", "+32 488 11 11 11", 2 },
                    { 4, "dr.Axl.Moreau@example.be", "Axl", "Moreau", "+32 488 22 22 22", 3 },
                    { 5, "dr.Peter.Mchealer@example.be", "Peter", "McHealer", "+32 499 33 33 33", 3 },
                    { 6, "dr.Kate.Grant@example.be", "Kate", "Grant", "+32 473 55 55 55", 3 },
                    { 7, "dr.Simon.DeJong@example.be", "Simon", "De Jong", "+32 474 66 66 22", 4 },
                    { 8, "dr.Bryan.Devries@example.be", "Bryan", "De Vries", "+32 475 77 77 77can", 5 }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 9999);

            migrationBuilder.DeleteData(
                table: "Doctors",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Doctors",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Doctors",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Doctors",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Doctors",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Doctors",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Doctors",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Doctors",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Specialties",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Specialties",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Specialties",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Specialties",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Specialties",
                keyColumn: "Id",
                keyValue: 5);
        }
    }
}
