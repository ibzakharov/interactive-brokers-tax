using Microsoft.EntityFrameworkCore.Migrations;

namespace Investing.Data.Migrations
{
    public partial class AddTaxCodeToCurrency : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TaxCode",
                table: "Currencies",
                type: "TEXT",
                nullable: true);
            migrationBuilder.Sql("update Currencies set TaxCode = '???'");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TaxCode",
                table: "Currencies");
        }
    }
}
