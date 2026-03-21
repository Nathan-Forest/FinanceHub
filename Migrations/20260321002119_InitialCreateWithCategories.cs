using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FinanceHub.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreateWithCategories : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Icon = table.Column<string>(type: "TEXT", nullable: false),
                    Color = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Transactions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Amount = table.Column<decimal>(type: "TEXT", nullable: false),
                    CategoryId = table.Column<int>(type: "INTEGER", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    Date = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Type = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Transactions_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "Color", "Icon", "Name" },
                values: new object[,]
                {
                    { 1, "#f59e0b", "🍕", "Food & Dining" },
                    { 2, "#3b82f6", "🚗", "Transportation" },
                    { 3, "#a855f7", "🎬", "Entertainment" },
                    { 4, "#ec4899", "🛍️", "Shopping" },
                    { 5, "#eab308", "💡", "Bills & Utilities" },
                    { 6, "#10b981", "🏥", "Health & Fitness" },
                    { 7, "#22c55e", "💰", "Income" },
                    { 8, "#6b7280", "📦", "Other" }
                });

            migrationBuilder.InsertData(
                table: "Transactions",
                columns: new[] { "Id", "Amount", "CategoryId", "Date", "Description", "Type" },
                values: new object[,]
                {
                    { 1, 50.00m, 1, new DateTime(2026, 3, 19, 10, 21, 19, 71, DateTimeKind.Local).AddTicks(9022), "Lunch at cafe", "expense" },
                    { 2, 2000.00m, 7, new DateTime(2026, 3, 16, 10, 21, 19, 71, DateTimeKind.Local).AddTicks(9033), "Monthly salary", "income" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_CategoryId",
                table: "Transactions",
                column: "CategoryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Transactions");

            migrationBuilder.DropTable(
                name: "Categories");
        }
    }
}
