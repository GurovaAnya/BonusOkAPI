using Microsoft.EntityFrameworkCore.Migrations;

namespace BonusOkAPI.Migrations
{
    public partial class KeyChanges : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropIndex(
              //  name: "IX_Clients_CardId",
                //table: "Clients");
                migrationBuilder.DropTable(
                    name: "ClientPromo");

            migrationBuilder.CreateTable(
                name: "ClientPromo",
                columns: table => new
                {
                    ClientsId = table.Column<int>(type: "int", nullable: false),
                    PromosId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientPromo", x => new { x.ClientsId, x.PromosId });
                    table.ForeignKey(
                        name: "FK_ClientPromo_Clients_ClientsId",
                        column: x => x.ClientsId,
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClientPromo_Promos_PromosId",
                        column: x => x.PromosId,
                        principalTable: "Promos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            //migrationBuilder.CreateIndex(
              //  name: "IX_Clients_CardId",
                //table: "Clients",
                //column: "CardId",
                //unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ClientPromo_PromosId",
                table: "ClientPromo",
                column: "PromosId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClientPromo");

            migrationBuilder.DropIndex(
                name: "IX_Clients_CardId",
                table: "Clients");

            migrationBuilder.CreateIndex(
                name: "IX_Clients_CardId",
                table: "Clients",
                column: "CardId");
        }
    }
}
