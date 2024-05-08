using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameStore.Users.Migrations;

/// <inheritdoc />
public partial class UniqueNames : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<string>(
            name: "Name",
            table: "AspNetUsers",
            type: "nvarchar(450)",
            nullable: false,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.CreateIndex(
            name: "IX_AspNetUsers_Name",
            table: "AspNetUsers",
            column: "Name",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_AspNetRoles_Name",
            table: "AspNetRoles",
            column: "Name",
            unique: true,
            filter: "[Name] IS NOT NULL");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "IX_AspNetUsers_Name",
            table: "AspNetUsers");

        migrationBuilder.DropIndex(
            name: "IX_AspNetRoles_Name",
            table: "AspNetRoles");

        migrationBuilder.AlterColumn<string>(
            name: "Name",
            table: "AspNetUsers",
            type: "nvarchar(max)",
            nullable: false,
            oldClrType: typeof(string),
            oldType: "nvarchar(450)");
    }
}
