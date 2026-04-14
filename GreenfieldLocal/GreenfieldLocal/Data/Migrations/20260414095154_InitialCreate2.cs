using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GreenfieldLocal.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "BasketProducts");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProductId",
                table: "BasketProducts",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
