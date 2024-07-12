using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameStore.Users.Migrations;

/// <inheritdoc />
public partial class Notifications : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "NotificationTypes",
            table: "AspNetUsers",
            type: "nvarchar(max)",
            nullable: false,
            defaultValue: "[]");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "NotificationTypes",
            table: "AspNetUsers");
    }
}
