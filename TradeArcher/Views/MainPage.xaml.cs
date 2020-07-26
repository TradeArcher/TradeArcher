using System;

using TradeArcher.ViewModels;

using Windows.UI.Xaml.Controls;

namespace TradeArcher.Views
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private MainViewModel ViewModel
        {
            get { return DataContext as MainViewModel; }
        }
    }
}
