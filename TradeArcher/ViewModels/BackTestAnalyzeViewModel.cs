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

        private double? _totalWins;
        public double? TotalWins
        {
            get => _totalWins;
            set => Set(ref _totalWins, value);
        }

        private double? _totalLosses;
        public double? TotalLosses
        {
            get => _totalLosses;
            set => Set(ref _totalLosses, value);
        }

        private int _winCount;
        public int WinCount
        {
            get => _winCount;
            set => Set(ref _winCount, value);
        }

        private int _lossCount;
        public int LossCount
        {
            get => _lossCount;
            set => Set(ref _lossCount, value);
        }

        private double? _avgWinAmt;
        public double? AvgWinAmt
        {
            get => _avgWinAmt;
            set => Set(ref _avgWinAmt, value);
        }

        private double? _avgLossAmt;
        public double? AvgLossAmt
        {
            get => _avgLossAmt;
            set => Set(ref _avgLossAmt, value);
        }

        private double? _biggestWin;
        public double? BiggestWin
        {
            get => _biggestWin;
            set => Set(ref _biggestWin, value);
        }

        private double? _biggestLoss;
        public double? BiggestLoss
        {
            get => _biggestLoss;
            set => Set(ref _biggestLoss, value);
        }

        private double? _profitFactor;
        public double? ProfitFactor
        {
            get => _profitFactor;
            set => Set(ref _profitFactor, value);
        }

        private double _winRate;
        public double WinRate
        {
            get => _winRate;
            set => Set(ref _winRate, value);
        }

        private double? _totalPnL;
        public double? TotalPnL
        {
            get => _totalPnL;
            set => Set(ref _totalPnL, value);
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

                    var gains = dbData.Where(t => (t.OrderSide == OrderSide.SellToClose || t.OrderSide == OrderSide.BuyToClose) && t.TickerSessionPnl >= 0).ToList();
                    var losses = dbData.Where(t => (t.OrderSide == OrderSide.SellToClose || t.OrderSide == OrderSide.BuyToClose) && t.TickerSessionPnl < 0).ToList();
                    TotalWins = gains.Sum(t => t.TradePnl);
                    TotalLosses = losses.Sum(t => t.TradePnl);
                    WinCount = gains.Count();
                    LossCount = losses.Count();
                    AvgWinAmt = gains.Average(t => t.TradePnl);
                    AvgLossAmt = losses.Average(t => t.TradePnl);
                    BiggestWin = gains.Max(t => t.TradePnl);
                    BiggestLoss = losses.Min(t => t.TradePnl);
                    ProfitFactor = TotalWins / TotalLosses;
                    WinRate = (double)WinCount / ((double)WinCount + (double)LossCount);

                    var profitsLossesByDay = dbData
                    .Where(t => t.OrderSide == OrderSide.SellToClose || t.OrderSide == OrderSide.BuyToClose)
                        .OrderBy(t => t.ExecutionTime)
                        .ThenBy(t => t.SymbolTradeId)
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

                    TotalPnL = ProfitLossData?.Last().PnL;

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
