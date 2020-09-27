using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Caliburn.Micro;
//using LiveCharts;
//using LiveCharts.Uwp;
using Microsoft.EntityFrameworkCore;
using TradeArcher.Core.Models;
using TradeArcher.Views;

namespace TradeArcher.ViewModels
{
    public class BackTestAnalyzeViewModel : Screen
    {
        public BackTestAnalyzeViewModel()
        {
        }

        public event EventHandler<int> ChartDataChanged;

        private ObservableCollection<Strategy> _strategies;
        public ObservableCollection<Strategy> Strategies
        {
            get => _strategies;
            set => Set(ref _strategies, value);
        }

        private Strategy _selectedStrategy;
        public Strategy SelectedStrategy
        {
            get => _selectedStrategy;
            set => Set(ref _selectedStrategy, value);
        }

        //private ObservableCollection<StrategyBackTestSession> _sessions;
        //public ObservableCollection<StrategyBackTestSession> Sessions
        //{
        //    get => _sessions;
        //    set => Set(ref _sessions, value);
        //}

        private StrategyBackTestSession _selectedSession;
        public StrategyBackTestSession SelectedSession
        {
            get => _selectedSession;
            set => Set(ref _selectedSession, value);
        }

        private ObservableCollection<PnLData> _profitLossData = new ObservableCollection<PnLData>();

        public ObservableCollection<PnLData> ProfitLossData
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

            UpdateStrategies();
            UpdateTrades();
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            PropertyChanged += OnPropertyChanged;
        }

        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);
            PropertyChanged -= OnPropertyChanged;
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(SelectedSession):
                case nameof(SelectedStrategy):
                    UpdateTrades();
                    break;
            }
        }


        private void UpdateStrategies()
        {
            using (var context = new TradeArcherDataContext())
            {
                Strategies = new ObservableCollection<Strategy>(context.Strategies.Include(s => s.Sessions).ThenInclude(s => s.BackTestTrades));
            }

            SelectedStrategy = Strategies.FirstOrDefault();
        }

        private void UpdateTrades()
        {
            ProfitLossData.Clear();

            if (SelectedStrategy != null)
            {
                using (var context = new TradeArcherDataContext())
                {
                    // TODO: group trades by stock and sort by date then calculate gain/loss by multiplying price by number of shares per trade then sum up and do it for buys and sells then subtract to calculate the total gain/loss
                    var dbData = context.BackTestTrades
                        .Include(t => t.StrategyBackTestSession)
                        .ThenInclude(s => s.Strategy)
                        .Where(t => t.StrategyBackTestSession.Strategy.StrategyId == SelectedStrategy.StrategyId)
                        .ToList();

                    if (SelectedSession != null)
                    {
                        dbData = dbData.Where(t => t.StrategyBackTestSession.StrategyBackTestSessionId == SelectedSession.StrategyBackTestSessionId).ToList();
                    }

                    var profitsLossesByDay = dbData
                    .Where(t => t.OrderSide == OrderSide.SellToClose || t.OrderSide == OrderSide.BuyToClose)
                        .OrderBy(t => t.ExecutionTime)
                        .GroupBy(t =>
                            t.ExecutionTime.ToString("MM/dd/yy"),
                            t =>
                                            t, (date, trades) =>
                                                new
                                                {
                                                    Date = date,
                                                    ProfitLossAmount = trades.Sum(t => t.TickerSessionPnl ?? 0)
                                                })
                        //.Select(k => new { Date = k.Key})
                        .ToList();

                    for (var ix = 0; ix < profitsLossesByDay.Count; ix++)
                    {
                        var lastPnlAmt = (ix > 0) ? ProfitLossData[ix - 1].PnL : 0;
                        var pnl = profitsLossesByDay[ix];
                        ProfitLossData.Add(new PnLData
                        {
                            Date = pnl.Date,
                            PnL = pnl.ProfitLossAmount + lastPnlAmt
                        });
                    }

                    ChartDataChanged?.Invoke(this, ProfitLossData.Count);
                }
            }
        }
    }

    public class BackTestPnLData
    {
        public string Date { get; set; }
        public double PnL { get; set; }
    }
}
