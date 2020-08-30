using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TradeArcher.Core.Data.Migrations
{
    public partial class InitialCreation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Brokers",
                columns: table => new
                {
                    BrokerId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Brokers", x => x.BrokerId);
                });

            migrationBuilder.CreateTable(
                name: "Accounts",
                columns: table => new
                {
                    AccountId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(maxLength: 255, nullable: true),
                    DisplayName = table.Column<string>(maxLength: 255, nullable: true),
                    BrokerId1 = table.Column<int>(nullable: false),
                    BrokerId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accounts", x => x.AccountId);
                    table.ForeignKey(
                        name: "FK_Accounts_Brokers_BrokerId",
                        column: x => x.BrokerId,
                        principalTable: "Brokers",
                        principalColumn: "BrokerId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Accounts_Brokers_BrokerId1",
                        column: x => x.BrokerId1,
                        principalTable: "Brokers",
                        principalColumn: "BrokerId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TradeHistory",
                columns: table => new
                {
                    TradeId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ExecutionTime = table.Column<DateTime>(nullable: false),
                    Expiration = table.Column<DateTime>(nullable: false),
                    Spread = table.Column<int>(nullable: false),
                    OrderSide = table.Column<int>(nullable: false),
                    Quantity = table.Column<double>(nullable: false),
                    Symbol = table.Column<string>(maxLength: 10, nullable: false),
                    Price = table.Column<double>(nullable: false),
                    NetPrice = table.Column<double>(nullable: false),
                    TradeType = table.Column<int>(nullable: false),
                    OrderType = table.Column<int>(nullable: false),
                    Strike = table.Column<string>(maxLength: 255, nullable: true),
                    PosEffect = table.Column<string>(maxLength: 255, nullable: true),
                    AccountId1 = table.Column<int>(nullable: false),
                    AccountId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TradeHistory", x => x.TradeId);
                    table.ForeignKey(
                        name: "FK_TradeHistory_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TradeHistory_Accounts_AccountId1",
                        column: x => x.AccountId1,
                        principalTable: "Accounts",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Brokers",
                columns: new[] { "BrokerId", "Name" },
                values: new object[] { 1, "TD Ameritrade" });

            migrationBuilder.InsertData(
                table: "Brokers",
                columns: new[] { "BrokerId", "Name" },
                values: new object[] { 2, "ThinkOrSwim" });

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_BrokerId",
                table: "Accounts",
                column: "BrokerId");

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_BrokerId1",
                table: "Accounts",
                column: "BrokerId1");

            migrationBuilder.CreateIndex(
                name: "IX_TradeHistory_AccountId",
                table: "TradeHistory",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_TradeHistory_AccountId1",
                table: "TradeHistory",
                column: "AccountId1");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TradeHistory");

            migrationBuilder.DropTable(
                name: "Accounts");

            migrationBuilder.DropTable(
                name: "Brokers");
        }
    }
}
