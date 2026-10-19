using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Exam.App.Migrations
{
    /// <inheritdoc />
    public partial class AddAppointmentReport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Anamnesis",
                table: "Appointments",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ReportCreatedAt",
                table: "Appointments",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Weight",
                table: "Appointments",
                type: "double precision",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Anamnesis",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "ReportCreatedAt",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "Weight",
                table: "Appointments");
        }
    }
}
