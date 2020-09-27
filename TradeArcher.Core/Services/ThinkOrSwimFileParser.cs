using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using TradeArcher.Core.Helpers;
using TradeArcher.Core.Models;

namespace TradeArcher.Core.Services
{
    public class ThinkOrSwimFileParser : IFileParser<IList<Trade>>
    {
        public IList<Trade> ParseFile(byte[] fileData)
        {
            var trades = new List<Trade>();

            using (var memoryStream = new MemoryStream(fileData))
            {
                using (var reader = new StreamReader(memoryStream, true))
                {
                    using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                    {
                        csv.Configuration.RegisterClassMap<ThinkOrSwimTradeMap>();
                        //csv.Configuration.TypeConverterOptionsCache.GetOptions<DateTime>().Formats = new[] { "MM/dd/yyyy HH:mm:ss" };
                        //csv.Configuration.CultureInfo = CultureInfo.GetCultureInfo("en-US");

                        var startTradesSection = false;
                        var isRowHeader = true;

                        while (csv.Read())
                        {
                            if (!startTradesSection)
                            {
                                var field = csv.GetField(0);
                                if (field == "Account Trade History")
                                {
                                    startTradesSection = true;
                                }
                                continue;
                            }

                            if (isRowHeader)
                            {
                                csv.ReadHeader();
                                isRowHeader = false;
                                continue;
                            }

                            if (string.IsNullOrEmpty(csv.GetField(0)) && string.IsNullOrEmpty(csv.GetField(1)))
                            {
                                isRowHeader = true;
                                continue;
                            }

                            if (trades.Count > 0 && !string.IsNullOrEmpty(csv.GetField(0)))
                            {
                                //Importing of trades is complete.  Ingore the rest of the document.
                                break;
                            }

                            if (csv.Context.HeaderRecord.Contains("Exec Time") && csv.Context.HeaderRecord.Contains("Side"))
                            {
                                try
                                {
                                    trades.Add(csv.GetRecord<Trade>());
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
            return trades;
        }
    }

    public sealed class ThinkOrSwimTradeMap : ClassMap<Trade>
    {
        public ThinkOrSwimTradeMap()
        {
            Map(m => m.ExecutionTime).Name("Exec Time").TypeConverter<CustomDateTimeConverter>();
            Map(m => m.Expiration).Name("Exp").TypeConverter<CustomNullableDateTimeConverter>();
            Map(m => m.NetPrice).Name("Net Price");
            Map(m => m.OrderSide).Name("Side").TypeConverter<CustomOrderSideEnumConverter>();
            Map(m => m.OrderType).Name("Order Type").TypeConverter<CustomOrderTypeEnumConverter>();
            Map(m => m.PosEffect).Name("Pos Effect");
            Map(m => m.Price);
            Map(m => m.Quantity).Name("Qty");
            Map(m => m.Spread);
            Map(m => m.Strike);
            Map(m => m.Symbol);
            Map(m => m.TradeType).Name("Type").TypeConverter<CustomTradeTypeEnumConverter>();
        }
    }

    public sealed class CustomDateTimeConverter : DateTimeConverter
    {
        public override object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return DateTime.MinValue;
            }

            var cultureInfo = new CultureInfo("en-US");
            cultureInfo.Calendar.TwoDigitYearMax = 2099;
            return DateTime.ParseExact(text, Constants.SupportedDateFormats, cultureInfo, DateTimeStyles.None);
        }
    }

    public sealed class CustomNullableDateTimeConverter : DateTimeConverter
    {
        public override object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return null;
            }

            var cultureInfo = new CultureInfo("en-US");
            cultureInfo.Calendar.TwoDigitYearMax = 2099;
            return DateTime.ParseExact(text, Constants.SupportedDateFormats, cultureInfo, DateTimeStyles.None);
        }
    }

    public sealed class CustomEnumConverter<T> : CsvHelper.TypeConversion.EnumConverter where T : struct
    {
        public CustomEnumConverter(): base(typeof(T)) { }

        public override object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
        {
            int parsedValue;

            if (int.TryParse(text, out parsedValue))
            {
                return (T)(object)parsedValue;
            }

            return base.ConvertFromString(text, row, memberMapData);
        }

        public override string ConvertToString(object value, IWriterRow row, MemberMapData memberMapData)
        {
            T result;

            if (Enum.TryParse(value.ToString(), out result))
            {
                return (Convert.ToInt32(result)).ToString();
            }

            return base.ConvertToString(value, row, memberMapData);
        }
    }

    public sealed class CustomOrderTypeEnumConverter : DefaultTypeConverter
    {
        public override object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return OrderType.Unknown;
            }

            switch (text.ToUpper())
            {
                case "LMT":
                    return OrderType.Limit;
                case "MKT":
                    return OrderType.Market;
                case "STP":
                    return OrderType.Stop;
                case "SLM":
                    return OrderType.StopLimit;
                default:
                    return OrderType.Unknown;
            }
        }
    }

    public sealed class CustomOrderSideEnumConverter : DefaultTypeConverter
    {
        public override object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return OrderSide.Unknown;
            }

            switch (text.ToUpper())
            {
                case "BUY":
                    return OrderSide.BuyToOpen;
                case "SELL":
                    return OrderSide.SellToClose;
                default:
                    return OrderSide.Unknown;
            }
        }
    }

    public sealed class CustomTradeTypeEnumConverter : DefaultTypeConverter
    {
        public override object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return TradeType.Unknown;
            }

            switch (text.ToUpper())
            {
                case "STOCK":
                    return TradeType.Stock;
                case "OPTION":
                    return TradeType.Option;
                case "FUTURE":
                    return TradeType.Future;
                case "FOREX":
                    return TradeType.Forex;
                default:
                    return TradeType.Unknown;
            }
        }
    }
}