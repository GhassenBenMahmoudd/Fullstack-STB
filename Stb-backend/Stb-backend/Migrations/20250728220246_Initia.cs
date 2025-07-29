using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace stb_backend.Migrations
{
    /// <inheritdoc />
    public partial class Initia : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DeclarationsCadeaux_User_IdUser",
                table: "DeclarationsCadeaux");

            migrationBuilder.DropForeignKey(
                name: "FK_DeclarationsCadeaux_User_ManagerIdUser",
                table: "DeclarationsCadeaux");

            migrationBuilder.DropForeignKey(
                name: "FK_DeclarationsCorruption_User_IdUser",
                table: "DeclarationsCorruption");

            migrationBuilder.DropForeignKey(
                name: "FK_DemandesConseil_User_IdUser",
                table: "DemandesConseil");

            migrationBuilder.DropColumn(
                name: "Departement",
                table: "User");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "User");

            migrationBuilder.DropColumn(
                name: "Matricule",
                table: "User");

            migrationBuilder.DropColumn(
                name: "Password",
                table: "User");

            migrationBuilder.CreateTable(
                name: "Employes",
                columns: table => new
                {
                    IdUser = table.Column<long>(type: "bigint", nullable: false),
                    Matricule = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Password = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employes", x => x.IdUser);
                    table.ForeignKey(
                        name: "FK_Employes_User_IdUser",
                        column: x => x.IdUser,
                        principalTable: "User",
                        principalColumn: "IdUser",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Managers",
                columns: table => new
                {
                    IdUser = table.Column<long>(type: "bigint", nullable: false),
                    Departement = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Managers", x => x.IdUser);
                    table.ForeignKey(
                        name: "FK_Managers_Employes_IdUser",
                        column: x => x.IdUser,
                        principalTable: "Employes",
                        principalColumn: "IdUser",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_DeclarationsCadeaux_Employes_IdUser",
                table: "DeclarationsCadeaux",
                column: "IdUser",
                principalTable: "Employes",
                principalColumn: "IdUser",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DeclarationsCadeaux_Managers_ManagerIdUser",
                table: "DeclarationsCadeaux",
                column: "ManagerIdUser",
                principalTable: "Managers",
                principalColumn: "IdUser");

            migrationBuilder.AddForeignKey(
                name: "FK_DeclarationsCorruption_Employes_IdUser",
                table: "DeclarationsCorruption",
                column: "IdUser",
                principalTable: "Employes",
                principalColumn: "IdUser",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DemandesConseil_Employes_IdUser",
                table: "DemandesConseil",
                column: "IdUser",
                principalTable: "Employes",
                principalColumn: "IdUser",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DeclarationsCadeaux_Employes_IdUser",
                table: "DeclarationsCadeaux");

            migrationBuilder.DropForeignKey(
                name: "FK_DeclarationsCadeaux_Managers_ManagerIdUser",
                table: "DeclarationsCadeaux");

            migrationBuilder.DropForeignKey(
                name: "FK_DeclarationsCorruption_Employes_IdUser",
                table: "DeclarationsCorruption");

            migrationBuilder.DropForeignKey(
                name: "FK_DemandesConseil_Employes_IdUser",
                table: "DemandesConseil");

            migrationBuilder.DropTable(
                name: "Managers");

            migrationBuilder.DropTable(
                name: "Employes");

            migrationBuilder.AddColumn<string>(
                name: "Departement",
                table: "User",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "User",
                type: "nvarchar(8)",
                maxLength: 8,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Matricule",
                table: "User",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Password",
                table: "User",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_DeclarationsCadeaux_User_IdUser",
                table: "DeclarationsCadeaux",
                column: "IdUser",
                principalTable: "User",
                principalColumn: "IdUser",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DeclarationsCadeaux_User_ManagerIdUser",
                table: "DeclarationsCadeaux",
                column: "ManagerIdUser",
                principalTable: "User",
                principalColumn: "IdUser");

            migrationBuilder.AddForeignKey(
                name: "FK_DeclarationsCorruption_User_IdUser",
                table: "DeclarationsCorruption",
                column: "IdUser",
                principalTable: "User",
                principalColumn: "IdUser",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DemandesConseil_User_IdUser",
                table: "DemandesConseil",
                column: "IdUser",
                principalTable: "User",
                principalColumn: "IdUser",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
