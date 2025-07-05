using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DimmedAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddIsActiveToInsurer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Insurer",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Insurer");
        }
    }
}
