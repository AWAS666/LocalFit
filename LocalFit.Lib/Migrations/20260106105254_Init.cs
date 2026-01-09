using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LocalFit.Lib.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FoodPresets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Calories = table.Column<decimal>(type: "TEXT", nullable: false),
                    Fat = table.Column<decimal>(type: "TEXT", nullable: false),
                    Protein = table.Column<decimal>(type: "TEXT", nullable: false),
                    Carbs = table.Column<decimal>(type: "TEXT", nullable: false),
                    Sugar = table.Column<decimal>(type: "TEXT", nullable: false),
                    Created = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Updated = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FoodPresets", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NutritionResult",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Calories = table.Column<decimal>(type: "TEXT", nullable: false),
                    ProteinGrams = table.Column<decimal>(type: "TEXT", nullable: false),
                    FatGrams = table.Column<decimal>(type: "TEXT", nullable: false),
                    CarbsGrams = table.Column<decimal>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NutritionResult", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Weight",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Height = table.Column<decimal>(type: "TEXT", nullable: false),
                    Weight = table.Column<decimal>(type: "TEXT", nullable: false),
                    Note = table.Column<string>(type: "TEXT", nullable: true),
                    Image = table.Column<byte[]>(type: "BLOB", nullable: true),
                    Created = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Updated = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Weight", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Day",
                columns: table => new
                {
                    Date = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    TargetsId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Day", x => x.Date);
                    table.ForeignKey(
                        name: "FK_Day_NutritionResult_TargetsId",
                        column: x => x.TargetsId,
                        principalTable: "NutritionResult",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FoodEaten",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Amount = table.Column<decimal>(type: "TEXT", nullable: false),
                    PresetId = table.Column<int>(type: "INTEGER", nullable: false),
                    Created = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Updated = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DayTrackerDate = table.Column<DateOnly>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FoodEaten", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FoodEaten_Day_DayTrackerDate",
                        column: x => x.DayTrackerDate,
                        principalTable: "Day",
                        principalColumn: "Date");
                    table.ForeignKey(
                        name: "FK_FoodEaten_FoodPresets_PresetId",
                        column: x => x.PresetId,
                        principalTable: "FoodPresets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Day_TargetsId",
                table: "Day",
                column: "TargetsId");

            migrationBuilder.CreateIndex(
                name: "IX_FoodEaten_DayTrackerDate",
                table: "FoodEaten",
                column: "DayTrackerDate");

            migrationBuilder.CreateIndex(
                name: "IX_FoodEaten_PresetId",
                table: "FoodEaten",
                column: "PresetId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FoodEaten");

            migrationBuilder.DropTable(
                name: "Weight");

            migrationBuilder.DropTable(
                name: "Day");

            migrationBuilder.DropTable(
                name: "FoodPresets");

            migrationBuilder.DropTable(
                name: "NutritionResult");
        }
    }
}
