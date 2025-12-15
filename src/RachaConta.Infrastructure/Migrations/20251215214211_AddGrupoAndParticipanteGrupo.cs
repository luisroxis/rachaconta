using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RachaConta.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddGrupoAndParticipanteGrupo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Invites_Users_UsuarioId",
                table: "Invites");

            migrationBuilder.DropIndex(
                name: "IX_Invites_UsuarioId",
                table: "Invites");

            migrationBuilder.DropColumn(
                name: "UsuarioId",
                table: "Invites");

            migrationBuilder.CreateTable(
                name: "Friendships",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    AmigoId = table.Column<Guid>(type: "uuid", nullable: false),
                    Approved = table.Column<bool>(type: "boolean", nullable: false),
                    Convidado = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    ConvidadoEmail = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    DataAprovacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Friendships", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Friendships_Users_AmigoId",
                        column: x => x.AmigoId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Friendships_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Grupos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    IdCategoria = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Nome = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Descricao = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    OutrasCategorias = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    LinkConvite = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Ativo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    Deleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    ImgGrupo = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Grupos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ParticipantesGrupo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    IdGrupo = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsAdm = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    HasPendent = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ParticipantesGrupo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ParticipantesGrupo_Grupos_IdGrupo",
                        column: x => x.IdGrupo,
                        principalTable: "Grupos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ParticipantesGrupo_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Friendships_AmigoId",
                table: "Friendships",
                column: "AmigoId");

            migrationBuilder.CreateIndex(
                name: "IX_Friendships_UserId",
                table: "Friendships",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Grupos_Ativo",
                table: "Grupos",
                column: "Ativo");

            migrationBuilder.CreateIndex(
                name: "IX_Grupos_Deleted",
                table: "Grupos",
                column: "Deleted");

            migrationBuilder.CreateIndex(
                name: "IX_ParticipantesGrupo_IdGrupo",
                table: "ParticipantesGrupo",
                column: "IdGrupo");

            migrationBuilder.CreateIndex(
                name: "IX_ParticipantesGrupo_IdGrupo_UserId",
                table: "ParticipantesGrupo",
                columns: new[] { "IdGrupo", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ParticipantesGrupo_UserId",
                table: "ParticipantesGrupo",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Friendships");

            migrationBuilder.DropTable(
                name: "ParticipantesGrupo");

            migrationBuilder.DropTable(
                name: "Grupos");

            migrationBuilder.AddColumn<Guid>(
                name: "UsuarioId",
                table: "Invites",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Invites_UsuarioId",
                table: "Invites",
                column: "UsuarioId");

            migrationBuilder.AddForeignKey(
                name: "FK_Invites_Users_UsuarioId",
                table: "Invites",
                column: "UsuarioId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
