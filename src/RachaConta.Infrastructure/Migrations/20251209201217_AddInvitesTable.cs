using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RachaConta.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddInvitesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Invites",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Nome = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Email = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    CorpoEmail = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    DataEnvio = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    AmigoId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Reenvio = table.Column<bool>(type: "boolean", nullable: false),
                    DataReenvio = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Aceite = table.Column<bool>(type: "boolean", nullable: false),
                    UsuarioId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Invites", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Invites_Users_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Invites_DataEnvio",
                table: "Invites",
                column: "DataEnvio");

            migrationBuilder.CreateIndex(
                name: "IX_Invites_Email",
                table: "Invites",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_Invites_UsuarioId",
                table: "Invites",
                column: "UsuarioId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Invites");
        }
    }
}
