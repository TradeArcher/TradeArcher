using System.Collections.ObjectModel;
using Caliburn.Micro;
using Microsoft.EntityFrameworkCore;
using TradeArcher.Core.Models;
using TradeArcher.Views;

namespace TradeArcher.ViewModels
{
    public class TradesViewModel : Screen
    {
        public TradesViewModel()
        {
        }

        private ObservableCollection<Trade> _trades;

        public ObservableCollection<Trade> Trades
        {
            get => _trades;
            set => Set(ref _trades, value);
        }

 
        protected override void OnInitialize()
        {
            base.OnInitialize();
            var view = GetView() as IShellView;

            UpdateTrades();
        }

        private void UpdateTrades()
        {
            using (var context = new TradeArcherDataContext())
            {
                Trades = new ObservableCollection<Trade>(context.TradeHistory.Include(t => t.Account).ThenInclude(a => a.Broker));
            }
        }
    }
}
