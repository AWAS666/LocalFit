using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LocalFit.Lib.Migrations
{
    /// <inheritdoc />
    public partial class Unit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Unit",
                table: "FoodPresets",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Unit",
                table: "FoodPresets");
        }
    }
}
