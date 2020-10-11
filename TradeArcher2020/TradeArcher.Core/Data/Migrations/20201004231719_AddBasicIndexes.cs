using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TradeArcher.Core.Data.Migrations
{
    public partial class AddBasicIndexes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Brokers",
                keyColumn: "BrokerId",
                keyValue: 1,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2020, 10, 4, 23, 17, 18, 800, DateTimeKind.Utc).AddTicks(5068), new DateTime(2020, 10, 4, 23, 17, 18, 800, DateTimeKind.Utc).AddTicks(5850) });

            migrationBuilder.UpdateData(
                table: "Brokers",
                keyColumn: "BrokerId",
                keyValue: 2,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2020, 10, 4, 23, 17, 18, 800, DateTimeKind.Utc).AddTicks(6459), new DateTime(2020, 10, 4, 23, 17, 18, 800, DateTimeKind.Utc).AddTicks(6469) });

            migrationBuilder.CreateIndex(
                name: "IX_TradeHistory_ExecutionTime",
                table: "TradeHistory",
                column: "ExecutionTime");

            migrationBuilder.CreateIndex(
                name: "IX_TradeHistory_OrderSide",
                table: "TradeHistory",
                column: "OrderSide");

            migrationBuilder.CreateIndex(
                name: "IX_TradeHistory_Price",
                table: "TradeHistory",
                column: "Price");

            migrationBuilder.CreateIndex(
                name: "IX_TradeHistory_Symbol",
                table: "TradeHistory",
                column: "Symbol");

            migrationBuilder.CreateIndex(
                name: "IX_StrategyBackTestSessions_Date",
                table: "StrategyBackTestSessions",
                column: "Date");

            migrationBuilder.CreateIndex(
                name: "IX_StrategyBackTestSessions_Name",
                table: "StrategyBackTestSessions",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Strategies_Name",
                table: "Strategies",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Brokers_Name",
                table: "Brokers",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_BackTestTrades_ExecutionTime",
                table: "BackTestTrades",
                column: "ExecutionTime");

            migrationBuilder.CreateIndex(
                name: "IX_BackTestTrades_OrderSide",
                table: "BackTestTrades",
                column: "OrderSide");

            migrationBuilder.CreateIndex(
                name: "IX_BackTestTrades_Price",
                table: "BackTestTrades",
                column: "Price");

            migrationBuilder.CreateIndex(
                name: "IX_BackTestTrades_Symbol",
                table: "BackTestTrades",
                column: "Symbol");

            migrationBuilder.CreateIndex(
                name: "IX_BackTestTrades_TickerSessionPnl",
                table: "BackTestTrades",
                column: "TickerSessionPnl");

            migrationBuilder.CreateIndex(
                name: "IX_BackTestTrades_TradePnl",
                table: "BackTestTrades",
                column: "TradePnl");

            migrationBuilder.CreateIndex(
                name: "IX_BackTestTrades_SymbolTradeId",
                table: "BackTestTrades",
                column: "SymbolTradeId");

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_Name",
                table: "Accounts",
                column: "Name");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TradeHistory_ExecutionTime",
                table: "TradeHistory");

            migrationBuilder.DropIndex(
                name: "IX_TradeHistory_OrderSide",
                table: "TradeHistory");

            migrationBuilder.DropIndex(
                name: "IX_TradeHistory_Price",
                table: "TradeHistory");

            migrationBuilder.DropIndex(
                name: "IX_TradeHistory_Symbol",
                table: "TradeHistory");

            migrationBuilder.DropIndex(
                name: "IX_StrategyBackTestSessions_Date",
                table: "StrategyBackTestSessions");

            migrationBuilder.DropIndex(
                name: "IX_StrategyBackTestSessions_Name",
                table: "StrategyBackTestSessions");

            migrationBuilder.DropIndex(
                name: "IX_Strategies_Name",
                table: "Strategies");

            migrationBuilder.DropIndex(
                name: "IX_Brokers_Name",
                table: "Brokers");

            migrationBuilder.DropIndex(
                name: "IX_BackTestTrades_ExecutionTime",
                table: "BackTestTrades");

            migrationBuilder.DropIndex(
                name: "IX_BackTestTrades_OrderSide",
                table: "BackTestTrades");

            migrationBuilder.DropIndex(
                name: "IX_BackTestTrades_Price",
                table: "BackTestTrades");

            migrationBuilder.DropIndex(
                name: "IX_BackTestTrades_Symbol",
                table: "BackTestTrades");

            migrationBuilder.DropIndex(
                name: "IX_BackTestTrades_TickerSessionPnl",
                table: "BackTestTrades");

            migrationBuilder.DropIndex(
                name: "IX_BackTestTrades_TradePnl",
                table: "BackTestTrades");

            migrationBuilder.DropIndex(
                name: "IX_BackTestTrades_SymbolTradeId",
                table: "BackTestTrades");

            migrationBuilder.DropIndex(
                name: "IX_Accounts_Name",
                table: "Accounts");

            migrationBuilder.UpdateData(
                table: "Brokers",
                keyColumn: "BrokerId",
                keyValue: 1,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2020, 9, 27, 2, 55, 42, 222, DateTimeKind.Utc).AddTicks(5933), new DateTime(2020, 9, 27, 2, 55, 42, 222, DateTimeKind.Utc).AddTicks(6805) });

            migrationBuilder.UpdateData(
                table: "Brokers",
                keyColumn: "BrokerId",
                keyValue: 2,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2020, 9, 27, 2, 55, 42, 222, DateTimeKind.Utc).AddTicks(7437), new DateTime(2020, 9, 27, 2, 55, 42, 222, DateTimeKind.Utc).AddTicks(7449) });
        }
    }
}
