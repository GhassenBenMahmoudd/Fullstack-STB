using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace stb_backend.Migrations
{
    /// <inheritdoc />
    public partial class AddEstArchiveToDeclarationCadeau11 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Archived",
                table: "DeclarationsCadeaux",
                newName: "EstArchive");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "EstArchive",
                table: "DeclarationsCadeaux",
                newName: "Archived");
        }
    }
}
