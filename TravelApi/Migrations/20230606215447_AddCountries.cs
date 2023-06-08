using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TravelApi.Migrations
{
    public partial class AddCountries : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Countries",
                columns: new[] { "CountryId", "Climate", "Language", "Name", "Population" },
                values: new object[] { 8, "Desert Island Vibe", "Nut", "Coconut Land", 2 });

            migrationBuilder.InsertData(
                table: "Countries",
                columns: new[] { "CountryId", "Climate", "Language", "Name", "Population" },
                values: new object[] { 9, "Glossy", "Sharpie", "Magnet Calendar", 7600 });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "CountryId",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "CountryId",
                keyValue: 9);
        }
    }
}
