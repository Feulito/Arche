using Microsoft.EntityFrameworkCore.Migrations;

namespace Database.Migrations
{
    public partial class Ajout_foreign_key_Auteur_Article : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Articles_AuteurId",
                table: "Articles",
                column: "AuteurId");

            migrationBuilder.AddForeignKey(
                name: "FK_Articles_Users_AuteurId",
                table: "Articles",
                column: "AuteurId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Articles_Users_AuteurId",
                table: "Articles");

            migrationBuilder.DropIndex(
                name: "IX_Articles_AuteurId",
                table: "Articles");
        }
    }
}
