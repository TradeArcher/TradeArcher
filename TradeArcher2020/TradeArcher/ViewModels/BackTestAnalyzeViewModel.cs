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

        private string _avgHoldTime;
        public string AvgHoldTime
        {
            get => _avgHoldTime;
            set => Set(ref _avgHoldTime, value);
        }

        private string _avgWinningHoldTime;
        public string AvgWinningHoldTime
        {
            get => _avgWinningHoldTime;
            set => Set(ref _avgWinningHoldTime, value);
        }

        private string _avgLosingHoldTime;
        public string AvgLosingHoldTime
        {
            get => _avgLosingHoldTime;
            set => Set(ref _avgLosingHoldTime, value);
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
                    UpdateTrades();
                    break;
                case nameof(SelectedStrategy):
                    SelectedSession = null;
                    UpdateTrades();
                    break;
            }
        }


        private void UpdateStrategies()
        {
            using (var context = new TradeArcherDataContext())
            {
                Strategies = new ObservableCollection<Strategy>(context.Strategies.Include(s => s.Sessions).ThenInclude(s => s.BackTestTrades).OrderBy(s => s.Name));
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
                        .Where(t => t.StrategyBackTestSession.Strategy.StrategyId == SelectedStrategy.StrategyId);

                    if (SelectedSession != null)
                    {
                        dbData = dbData.Where(t => t.StrategyBackTestSession.StrategyBackTestSessionId == SelectedSession.StrategyBackTestSessionId);
                    }

                    var allTrades = dbData.OrderBy(t => t.Symbol).ThenBy(t => t.SymbolTradeId);

                    var tradeTimes = new List<BackTestTradeTime>();

                    foreach (var backTestOpenTrade in allTrades.Where(t => t.OrderSide == OrderSide.BuyToOpen || t.OrderSide == OrderSide.SellToOpen))
                    {
                        var backTestCloseTrade = allTrades.FirstOrDefault(t => t.Symbol == backTestOpenTrade.Symbol && t.SymbolTradeId > backTestOpenTrade.SymbolTradeId && (t.OrderSide == OrderSide.BuyToClose || t.OrderSide == OrderSide.SellToClose));

                        if (backTestCloseTrade != null)
                        {
                            tradeTimes.Add(new BackTestTradeTime
                            {
                                Symbol = backTestOpenTrade.Symbol,
                                HoldTime = backTestCloseTrade.ExecutionTime - backTestOpenTrade.ExecutionTime,
                                IsWinner = backTestCloseTrade.TradePnl > 0
                            });
                        }
                    }

                    AvgHoldTime = TimeSpan.FromMilliseconds(tradeTimes.Average(t => t.HoldTime.TotalMilliseconds)).ToString("d'd 'h'h 'm'm 's's'");
                    AvgWinningHoldTime = TimeSpan.FromMilliseconds(tradeTimes.Any(t => t.IsWinner) ? tradeTimes.Where(t => t.IsWinner).Average(t => t.HoldTime.TotalMilliseconds) : 0).ToString("d'd 'h'h 'm'm 's's'");
                    AvgLosingHoldTime = TimeSpan.FromMilliseconds(tradeTimes.Any(t => !t.IsWinner) ? tradeTimes.Where(t => !t.IsWinner).Average(t => t.HoldTime.TotalMilliseconds) : 0).ToString("d'd 'h'h 'm'm 's's'");

                    var gains = dbData.Where(t => (t.OrderSide == OrderSide.SellToClose || t.OrderSide == OrderSide.BuyToClose) && t.TradePnl > 0).ToList();
                    var losses = dbData.Where(t => (t.OrderSide == OrderSide.SellToClose || t.OrderSide == OrderSide.BuyToClose) && t.TradePnl <= 0).ToList();
                    TotalWins = gains.Sum(t => t.TradePnl);
                    TotalLosses = losses.Sum(t => t.TradePnl);
                    WinCount = gains.Count();
                    LossCount = losses.Count();
                    AvgWinAmt = gains.Average(t => t.TradePnl);
                    AvgLossAmt = losses.Average(t => t.TradePnl);
                    BiggestWin = gains.Max(t => t.TradePnl);
                    BiggestLoss = losses.Min(t => t.TradePnl);
                    ProfitFactor = TotalWins / TotalLosses * -1;
                    WinRate = (double)WinCount / ((double)WinCount + (double)LossCount);

                    var profitsLossesByDay = dbData
                    .Where(t => t.OrderSide == OrderSide.SellToClose || t.OrderSide == OrderSide.BuyToClose)
                        .OrderBy(t => t.ExecutionTime)
                        .ThenBy(t => t.SymbolTradeId).ToList()
                        .GroupBy(t =>
                            t.ExecutionTime.ToString("MM/dd/yy"),
                            t =>
                                            t, (date, trades) =>
                                                new
                                                {
                                                    Date = date,
                                                    ProfitLossAmount = trades.Sum(t => t.TradePnl ?? 0)
                                                }).ToList();

                    double lastPnlAmt = 0;
                    for (var ix = 0; ix < profitsLossesByDay.Count; ix++)
                    {
                        var pnl = profitsLossesByDay[ix];
                        var newPnlData = new PnLData {Date = pnl.Date, PnL = pnl.ProfitLossAmount + lastPnlAmt};
                        ProfitLossData.Add(newPnlData);
                        lastPnlAmt = newPnlData.PnL;
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

    public class BackTestTradeTime
    {
        public string Symbol { get; set; }
        public bool IsWinner { get; set; }
        public TimeSpan HoldTime { get; set; }
    }
}
