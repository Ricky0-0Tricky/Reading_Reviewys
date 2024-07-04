using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Reading_Reviewys.Data.Migrations
{
    /// <inheritdoc />
    public partial class AdicaoUserID2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserID",
                table: "Utilizador",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserID",
                table: "Utilizador");
        }
    }
}
