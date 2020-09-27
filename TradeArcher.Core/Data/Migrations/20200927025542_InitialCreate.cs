using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TradeArcher.Core.Data.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Brokers",
                columns: table => new
                {
                    BrokerId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CreatedDate = table.Column<DateTime>(nullable: true),
                    ModifiedDate = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Brokers", x => x.BrokerId);
                });

            migrationBuilder.CreateTable(
                name: "Strategies",
                columns: table => new
                {
                    StrategyId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CreatedDate = table.Column<DateTime>(nullable: true),
                    ModifiedDate = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Strategies", x => x.StrategyId);
                });

            migrationBuilder.CreateTable(
                name: "Accounts",
                columns: table => new
                {
                    AccountId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CreatedDate = table.Column<DateTime>(nullable: true),
                    ModifiedDate = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(maxLength: 255, nullable: true),
                    DisplayName = table.Column<string>(maxLength: 255, nullable: true),
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
                });

            migrationBuilder.CreateTable(
                name: "StrategyBackTestSessions",
                columns: table => new
                {
                    StrategyBackTestSessionId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CreatedDate = table.Column<DateTime>(nullable: true),
                    ModifiedDate = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(maxLength: 255, nullable: false),
                    Description = table.Column<string>(maxLength: 255, nullable: true),
                    Date = table.Column<DateTime>(nullable: false),
                    StrategyId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StrategyBackTestSessions", x => x.StrategyBackTestSessionId);
                    table.ForeignKey(
                        name: "FK_StrategyBackTestSessions_Strategies_StrategyId",
                        column: x => x.StrategyId,
                        principalTable: "Strategies",
                        principalColumn: "StrategyId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TradeHistory",
                columns: table => new
                {
                    TradeId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CreatedDate = table.Column<DateTime>(nullable: true),
                    ModifiedDate = table.Column<DateTime>(nullable: true),
                    ExecutionTime = table.Column<DateTime>(nullable: false),
                    Expiration = table.Column<DateTime>(nullable: true),
                    Spread = table.Column<int>(nullable: false),
                    OrderSide = table.Column<int>(nullable: false),
                    Quantity = table.Column<int>(nullable: false),
                    Symbol = table.Column<string>(maxLength: 10, nullable: false),
                    Price = table.Column<double>(nullable: false),
                    NetPrice = table.Column<double>(nullable: false),
                    TradeType = table.Column<int>(nullable: false),
                    OrderType = table.Column<int>(nullable: false),
                    Strike = table.Column<string>(maxLength: 255, nullable: true),
                    PosEffect = table.Column<string>(maxLength: 255, nullable: true),
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
                });

            migrationBuilder.CreateTable(
                name: "BackTestTrades",
                columns: table => new
                {
                    BackTestTradeId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CreatedDate = table.Column<DateTime>(nullable: true),
                    ModifiedDate = table.Column<DateTime>(nullable: true),
                    SymbolTradeId = table.Column<int>(nullable: false),
                    StrategyFullName = table.Column<string>(maxLength: 255, nullable: false),
                    Symbol = table.Column<string>(maxLength: 10, nullable: false),
                    OrderSide = table.Column<int>(nullable: false),
                    Quantity = table.Column<double>(nullable: false),
                    Price = table.Column<double>(nullable: false),
                    ExecutionTime = table.Column<DateTime>(nullable: false),
                    TradePnl = table.Column<double>(nullable: true),
                    TickerSessionPnl = table.Column<double>(nullable: true),
                    TickerSessionPosition = table.Column<double>(nullable: false),
                    StrategyBackTestSessionId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BackTestTrades", x => x.BackTestTradeId);
                    table.ForeignKey(
                        name: "FK_BackTestTrades_StrategyBackTestSessions_StrategyBackTestSessionId",
                        column: x => x.StrategyBackTestSessionId,
                        principalTable: "StrategyBackTestSessions",
                        principalColumn: "StrategyBackTestSessionId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Brokers",
                columns: new[] { "BrokerId", "CreatedDate", "ModifiedDate", "Name" },
                values: new object[] { 1, new DateTime(2020, 9, 27, 2, 55, 42, 222, DateTimeKind.Utc).AddTicks(5933), new DateTime(2020, 9, 27, 2, 55, 42, 222, DateTimeKind.Utc).AddTicks(6805), "TD Ameritrade" });

            migrationBuilder.InsertData(
                table: "Brokers",
                columns: new[] { "BrokerId", "CreatedDate", "ModifiedDate", "Name" },
                values: new object[] { 2, new DateTime(2020, 9, 27, 2, 55, 42, 222, DateTimeKind.Utc).AddTicks(7437), new DateTime(2020, 9, 27, 2, 55, 42, 222, DateTimeKind.Utc).AddTicks(7449), "ThinkOrSwim" });

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_BrokerId",
                table: "Accounts",
                column: "BrokerId");

            migrationBuilder.CreateIndex(
                name: "IX_BackTestTrades_StrategyBackTestSessionId",
                table: "BackTestTrades",
                column: "StrategyBackTestSessionId");

            migrationBuilder.CreateIndex(
                name: "IX_StrategyBackTestSessions_StrategyId",
                table: "StrategyBackTestSessions",
                column: "StrategyId");

            migrationBuilder.CreateIndex(
                name: "IX_TradeHistory_AccountId",
                table: "TradeHistory",
                column: "AccountId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BackTestTrades");

            migrationBuilder.DropTable(
                name: "TradeHistory");

            migrationBuilder.DropTable(
                name: "StrategyBackTestSessions");

            migrationBuilder.DropTable(
                name: "Accounts");

            migrationBuilder.DropTable(
                name: "Strategies");

            migrationBuilder.DropTable(
                name: "Brokers");
        }
    }
}
