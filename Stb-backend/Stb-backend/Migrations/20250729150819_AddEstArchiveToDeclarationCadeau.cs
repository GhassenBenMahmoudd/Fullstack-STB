using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace stb_backend.Migrations
{
    /// <inheritdoc />
    public partial class AddEstArchiveToDeclarationCadeau : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Archived",
                table: "DeclarationsCadeaux",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Archived",
                table: "DeclarationsCadeaux");
        }
    }
}
