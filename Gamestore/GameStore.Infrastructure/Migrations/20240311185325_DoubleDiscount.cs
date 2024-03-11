using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameStore.Infrastructure.Migrations;

/// <inheritdoc />
public partial class DoubleDiscount : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<double>(
            name: "Discount",
            table: "OrderGames",
            type: "float",
            nullable: false,
            oldClrType: typeof(int),
            oldType: "int");

        migrationBuilder.AlterColumn<double>(
            name: "Discount",
            table: "Games",
            type: "float",
            nullable: false,
            oldClrType: typeof(int),
            oldType: "int");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<int>(
            name: "Discount",
            table: "OrderGames",
            type: "int",
            nullable: false,
            oldClrType: typeof(double),
            oldType: "float");

        migrationBuilder.AlterColumn<int>(
            name: "Discount",
            table: "Games",
            type: "int",
            nullable: false,
            oldClrType: typeof(double),
            oldType: "float");
    }
}
