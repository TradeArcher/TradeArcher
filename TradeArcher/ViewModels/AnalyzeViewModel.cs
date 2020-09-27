using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Caliburn.Micro;
//using LiveCharts;
//using LiveCharts.Uwp;
using Microsoft.EntityFrameworkCore;
using TradeArcher.Core.Models;
using TradeArcher.Views;

namespace TradeArcher.ViewModels
{
    public class AnalyzeViewModel : Screen
    {
        public AnalyzeViewModel()
        {
        }

        private List<PnLData> _profitLossData;

        public List<PnLData> ProfitLossData
        {
            get => _profitLossData;
            set => Set(ref _profitLossData, value);
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            var view = GetView() as IShellView;

            //ProfitLossData = new SeriesCollection
            //{
            //    new LineSeries()
            //    {
            //        Title = "Profit / Loss"
            //    }
            //};

            UpdateTrades();
        }

        private void UpdateTrades()
        {
            using (var context = new TradeArcherDataContext())
            {
                // TODO: group trades by stock and sort by date then calculate gain/loss by multiplying price by number of shares per trade then sum up and do it for buys and sells then subtract to calculate the total gain/loss
                var profitsLossesByDay = context.TradeHistory
                    .Include(t => t.Account)
                    .ThenInclude(a => a.Broker)
                    .OrderBy(t => t.ExecutionTime)
                    .AsEnumerable()
                    //.Where(t => t.OrderSide == OrderSide.Sell)
                    .GroupBy(t => t.ExecutionTime.ToShortDateString(),
                        t => t,
                        (date, trades) => new
                        {
                            Date = date,
                            ProfitLossAmount = trades.Where(t => t.OrderSide == OrderSide.SellToClose).Select(t => t.Price).Sum() - trades.Where(t => t.OrderSide == OrderSide.BuyToOpen).Select(t => t.Price).Sum()
                        })
                    //.Select(k => new { Date = k.Key})
                    .ToList();

                ProfitLossData = profitsLossesByDay.Select(pnl => new PnLData {Date = pnl.Date, PnL = pnl.ProfitLossAmount}).ToList();
            }
        }
    }

    public class PnLData
    {
        public string Date { get; set; }
        public double PnL { get; set; }
    }
}
