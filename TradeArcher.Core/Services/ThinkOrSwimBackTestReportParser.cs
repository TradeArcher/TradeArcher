using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using TradeArcher.Core.Models;

namespace TradeArcher.Core.Services
{
    public class ThinkOrSwimBackTestReportParser : IFileParser<StrategyBackTestSession>
    {
        const string SymbolFieldPrefix = "Symbol:";
        const string WorkTimeFieldPrefix = "Work Time:";
        private const string TradesSectionHeaderIdFieldName = "Id";
        private const string TradesSectionHeaderStrategyFieldName = "Strategy";

        public StrategyBackTestSession ParseFile(byte[] fileData)
        {
            StrategyBackTestSession session = new StrategyBackTestSession();
            string symbol = null;

            using (var memoryStream = new MemoryStream(fileData))
            {
                using (var reader = new StreamReader(memoryStream, true))
                {
                    using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                    {
                        //For some reason ThinkOrSwim generates a CSV file with semicolons instead of commas for this particular report and not regular trade exports.
                        csv.Configuration.Delimiter = ";";
                        csv.Configuration.RegisterClassMap<ThinkOrSwimBackTestReportMap>();

                        var isInTradesSection = false;

                        while (csv.Read())
                        {
                            if (!isInTradesSection)
                            {
                                var field = csv.GetField(0);
                                if (!string.IsNullOrWhiteSpace(field) && field.StartsWith(SymbolFieldPrefix))
                                {
                                    symbol = field.TrimStart(SymbolFieldPrefix.ToCharArray());
                                    continue;
                                }
                                else if (!string.IsNullOrWhiteSpace(field) && field.StartsWith(TradesSectionHeaderIdFieldName))
                                {
                                    isInTradesSection = true;
                                    csv.ReadHeader();
                                    continue;
                                }
                                else
                                {
                                    continue;
                                }
                            }

                            if (csv.Context.Record.Length < 3)
                            {
                                break;
                            }


                            if (csv.Context.HeaderRecord.Contains(TradesSectionHeaderIdFieldName) && csv.Context.HeaderRecord.Contains(TradesSectionHeaderStrategyFieldName))
                            {
                                try
                                {
                                    var backTestTrade = csv.GetRecord<BackTestTrade>();
                                    backTestTrade.Symbol = symbol;
                                    session.BackTestTrades.Add(backTestTrade);
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine(e);
                                    throw;
                                }
                            }
                        }
                    }
                }
            }
            return session;
        }
    }

    public sealed class ThinkOrSwimBackTestReportMap : ClassMap<BackTestTrade>
    {
        public ThinkOrSwimBackTestReportMap()
        {
            Map(m => m.SymbolTradeId).Name("Id");
            Map(m => m.StrategyFullName).Name("Strategy");
            Map(m => m.OrderSide).Name("Side").TypeConverter<CustomBackTestOrderSideEnumConverter>();
            Map(m => m.Quantity).Name("Amount");
            Map(m => m.Price).ConvertUsing(row => double.Parse(row.GetField("Price"), NumberStyles.Currency));
            Map(m => m.ExecutionTime).Name("Date/Time").TypeConverter<CustomDateTimeConverter>();
            Map(m => m.TradePnl).Name("Trade P/L").ConvertUsing(row =>
            {
                if (double.TryParse(row.GetField("Trade P/L"), NumberStyles.Currency, CultureInfo.CurrentCulture, out double result))
                {
                    return result;
                }

                return null;
            });
            Map(m => m.TickerSessionPnl).Name("P/L").ConvertUsing(row => double.Parse(row.GetField("P/L"), NumberStyles.Currency));
            Map(m => m.TickerSessionPosition).Name("Position");
        }
    }

    public sealed class CustomBackTestOrderSideEnumConverter : DefaultTypeConverter
    {
        public override object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return OrderSide.Unknown;
            }

            switch (text.ToUpper())
            {
                case "BUY TO OPEN":
                    return OrderSide.BuyToOpen;
                case "SELL TO OPEN":
                    return OrderSide.SellToOpen;
                case "BUY TO CLOSE":
                    return OrderSide.BuyToClose;
                case "SELL TO CLOSE":
                    return OrderSide.SellToClose;
                default:
                    return OrderSide.Unknown;
            }
        }
    }
}