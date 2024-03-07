using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameStore.Infrastructure.Migrations;

/// <inheritdoc />
#pragma warning disable CA1707 // Identifiers should not contain underscores
public partial class Epic6_Genre_update : Migration
#pragma warning restore CA1707 // Identifiers should not contain underscores
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "Description",
            table: "Genres",
            type: "nvarchar(max)",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "Picture",
            table: "Genres",
            type: "nvarchar(max)",
            nullable: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "Description",
            table: "Genres");

        migrationBuilder.DropColumn(
            name: "Picture",
            table: "Genres");
    }
}
