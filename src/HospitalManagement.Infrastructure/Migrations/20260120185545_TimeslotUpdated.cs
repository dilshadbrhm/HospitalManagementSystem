using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HospitalManagement.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class TimeslotUpdated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Location",
                table: "TimeSlots",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Location",
                table: "TimeSlots");
        }
    }
}
