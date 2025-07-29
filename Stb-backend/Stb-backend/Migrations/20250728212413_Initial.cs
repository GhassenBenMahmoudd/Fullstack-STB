using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace stb_backend.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    IdUser = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Prenom = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Nom = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NumeroTelephone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Discriminator = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: false),
                    Matricule = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Password = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Departement = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.IdUser);
                });

            migrationBuilder.CreateTable(
                name: "DeclarationsCadeaux",
                columns: table => new
                {
                    IdCadeaux = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdUser = table.Column<long>(type: "bigint", nullable: false),
                    GUID = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ValeurEstime = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IdentiteDonneur = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TypeRelation = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Occasion = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Honneur = table.Column<bool>(type: "bit", nullable: false),
                    DateDeclaration = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Statut = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateReceptionCadeaux = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Anonyme = table.Column<bool>(type: "bit", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    ManagerIdUser = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeclarationsCadeaux", x => x.IdCadeaux);
                    table.ForeignKey(
                        name: "FK_DeclarationsCadeaux_User_IdUser",
                        column: x => x.IdUser,
                        principalTable: "User",
                        principalColumn: "IdUser",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DeclarationsCadeaux_User_ManagerIdUser",
                        column: x => x.ManagerIdUser,
                        principalTable: "User",
                        principalColumn: "IdUser");
                });

            migrationBuilder.CreateTable(
                name: "DeclarationsCorruption",
                columns: table => new
                {
                    IdCorruption = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdUser = table.Column<long>(type: "bigint", nullable: false),
                    ObjetSignalement = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    EntitesConcernees = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    DateObservation = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TypeDomaine = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Statut = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Anonyme = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeclarationsCorruption", x => x.IdCorruption);
                    table.ForeignKey(
                        name: "FK_DeclarationsCorruption_User_IdUser",
                        column: x => x.IdUser,
                        principalTable: "User",
                        principalColumn: "IdUser",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DemandesConseil",
                columns: table => new
                {
                    IdConseil = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdUser = table.Column<long>(type: "bigint", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    DateDemande = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Objet = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Statut = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Anonyme = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DemandesConseil", x => x.IdConseil);
                    table.ForeignKey(
                        name: "FK_DemandesConseil_User_IdUser",
                        column: x => x.IdUser,
                        principalTable: "User",
                        principalColumn: "IdUser",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DocumentFile",
                columns: table => new
                {
                    IdFile = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdCorruption = table.Column<long>(type: "bigint", nullable: true),
                    IdCadeaux = table.Column<long>(type: "bigint", nullable: true),
                    IdConseil = table.Column<long>(type: "bigint", nullable: true),
                    FileName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    FilePath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    DateUpload = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeclarationCorruptionIdCorruption = table.Column<long>(type: "bigint", nullable: true),
                    DemandeConseilIdConseil = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentFile", x => x.IdFile);
                    table.ForeignKey(
                        name: "FK_DocumentFile_DeclarationsCadeaux_IdCadeaux",
                        column: x => x.IdCadeaux,
                        principalTable: "DeclarationsCadeaux",
                        principalColumn: "IdCadeaux",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DocumentFile_DeclarationsCorruption_DeclarationCorruptionIdCorruption",
                        column: x => x.DeclarationCorruptionIdCorruption,
                        principalTable: "DeclarationsCorruption",
                        principalColumn: "IdCorruption");
                    table.ForeignKey(
                        name: "FK_DocumentFile_DeclarationsCorruption_IdCorruption",
                        column: x => x.IdCorruption,
                        principalTable: "DeclarationsCorruption",
                        principalColumn: "IdCorruption");
                    table.ForeignKey(
                        name: "FK_DocumentFile_DemandesConseil_DemandeConseilIdConseil",
                        column: x => x.DemandeConseilIdConseil,
                        principalTable: "DemandesConseil",
                        principalColumn: "IdConseil");
                    table.ForeignKey(
                        name: "FK_DocumentFile_DemandesConseil_IdConseil",
                        column: x => x.IdConseil,
                        principalTable: "DemandesConseil",
                        principalColumn: "IdConseil");
                });

            migrationBuilder.CreateIndex(
                name: "IX_DeclarationsCadeaux_IdUser",
                table: "DeclarationsCadeaux",
                column: "IdUser");

            migrationBuilder.CreateIndex(
                name: "IX_DeclarationsCadeaux_ManagerIdUser",
                table: "DeclarationsCadeaux",
                column: "ManagerIdUser");

            migrationBuilder.CreateIndex(
                name: "IX_DeclarationsCorruption_IdUser",
                table: "DeclarationsCorruption",
                column: "IdUser");

            migrationBuilder.CreateIndex(
                name: "IX_DemandesConseil_IdUser",
                table: "DemandesConseil",
                column: "IdUser");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentFile_DeclarationCorruptionIdCorruption",
                table: "DocumentFile",
                column: "DeclarationCorruptionIdCorruption");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentFile_DemandeConseilIdConseil",
                table: "DocumentFile",
                column: "DemandeConseilIdConseil");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentFile_IdCadeaux",
                table: "DocumentFile",
                column: "IdCadeaux");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentFile_IdConseil",
                table: "DocumentFile",
                column: "IdConseil");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentFile_IdCorruption",
                table: "DocumentFile",
                column: "IdCorruption");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DocumentFile");

            migrationBuilder.DropTable(
                name: "DeclarationsCadeaux");

            migrationBuilder.DropTable(
                name: "DeclarationsCorruption");

            migrationBuilder.DropTable(
                name: "DemandesConseil");

            migrationBuilder.DropTable(
                name: "User");
        }
    }
}
