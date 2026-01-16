using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LocalFit.Lib.Migrations
{
    /// <inheritdoc />
    public partial class AddIsDeletedToFoodPresets : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "FoodPresets",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "FoodPresets");
        }
    }
}
