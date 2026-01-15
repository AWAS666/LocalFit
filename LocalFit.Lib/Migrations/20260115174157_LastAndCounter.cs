using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LocalFit.Lib.Migrations
{
    /// <inheritdoc />
    public partial class LastAndCounter : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "LastAmount",
                table: "FoodPresets",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UseCount",
                table: "FoodPresets",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastAmount",
                table: "FoodPresets");

            migrationBuilder.DropColumn(
                name: "UseCount",
                table: "FoodPresets");
        }
    }
}
