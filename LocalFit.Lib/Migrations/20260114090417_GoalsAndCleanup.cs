using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LocalFit.Lib.Migrations
{
    /// <inheritdoc />
    public partial class GoalsAndCleanup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Day_NutritionResult_TargetsId",
                table: "Day");

            migrationBuilder.DropTable(
                name: "NutritionResult");

            migrationBuilder.DropIndex(
                name: "IX_Day_TargetsId",
                table: "Day");

            migrationBuilder.DropColumn(
                name: "TargetsId",
                table: "Day");

            migrationBuilder.AddColumn<int>(
                name: "GoalId",
                table: "Day",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "WeightId",
                table: "Day",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Goal",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Goal = table.Column<int>(type: "INTEGER", nullable: false),
                    FitLevel = table.Column<int>(type: "INTEGER", nullable: false),
                    Sex = table.Column<int>(type: "INTEGER", nullable: false),
                    Created = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Updated = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Goal", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Day_GoalId",
                table: "Day",
                column: "GoalId");

            migrationBuilder.CreateIndex(
                name: "IX_Day_WeightId",
                table: "Day",
                column: "WeightId");

            migrationBuilder.AddForeignKey(
                name: "FK_Day_Goal_GoalId",
                table: "Day",
                column: "GoalId",
                principalTable: "Goal",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Day_Weight_WeightId",
                table: "Day",
                column: "WeightId",
                principalTable: "Weight",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Day_Goal_GoalId",
                table: "Day");

            migrationBuilder.DropForeignKey(
                name: "FK_Day_Weight_WeightId",
                table: "Day");

            migrationBuilder.DropTable(
                name: "Goal");

            migrationBuilder.DropIndex(
                name: "IX_Day_GoalId",
                table: "Day");

            migrationBuilder.DropIndex(
                name: "IX_Day_WeightId",
                table: "Day");

            migrationBuilder.DropColumn(
                name: "GoalId",
                table: "Day");

            migrationBuilder.DropColumn(
                name: "WeightId",
                table: "Day");

            migrationBuilder.AddColumn<int>(
                name: "TargetsId",
                table: "Day",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "NutritionResult",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Calories = table.Column<decimal>(type: "TEXT", nullable: false),
                    CarbsGrams = table.Column<decimal>(type: "TEXT", nullable: false),
                    FatGrams = table.Column<decimal>(type: "TEXT", nullable: false),
                    ProteinGrams = table.Column<decimal>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NutritionResult", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Day_TargetsId",
                table: "Day",
                column: "TargetsId");

            migrationBuilder.AddForeignKey(
                name: "FK_Day_NutritionResult_TargetsId",
                table: "Day",
                column: "TargetsId",
                principalTable: "NutritionResult",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
