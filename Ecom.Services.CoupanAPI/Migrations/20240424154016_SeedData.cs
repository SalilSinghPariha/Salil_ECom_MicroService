using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Ecom.Services.CoupanAPI.Migrations
{
    /// <inheritdoc />
    public partial class SeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "coupans",
                columns: new[] { "CoupanId", "CoupanCode", "DiscountAmount", "MinAmount" },
                values: new object[,]
                {
                    { 1, "10OFF", 10.0, 20 },
                    { 2, "20OFF", 20.0, 40 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "coupans",
                keyColumn: "CoupanId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "coupans",
                keyColumn: "CoupanId",
                keyValue: 2);
        }
    }
}
