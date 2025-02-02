using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FirstBrickAPI.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePortfolioSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "investment_total",
                table: "portfolios",
                newName: "amount");

            migrationBuilder.AddColumn<DateTime>(
                name: "invested_at",
                table: "portfolios",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "updated_at",
                table: "portfolios",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "invested_at",
                table: "portfolios");

            migrationBuilder.DropColumn(
                name: "updated_at",
                table: "portfolios");

            migrationBuilder.RenameColumn(
                name: "amount",
                table: "portfolios",
                newName: "investment_total");
        }
    }
}
