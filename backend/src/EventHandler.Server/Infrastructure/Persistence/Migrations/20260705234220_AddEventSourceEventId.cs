using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventHandler.Server.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddEventSourceEventId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SourceEventId",
                table: "Events",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SourceEventId",
                table: "Events");
        }
    }
}
