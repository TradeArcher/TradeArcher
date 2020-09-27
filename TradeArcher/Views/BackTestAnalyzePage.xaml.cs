using System;
using System.Collections.Specialized;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using TradeArcher.ViewModels;
using WinRTXamlToolkit.Controls.DataVisualization.Charting;
using WinRTXamlToolkit.Controls.DataVisualization.Charting.Primitives;
using WinRTXamlToolkit.Controls.Extensions;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace TradeArcher.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class BackTestAnalyzePage : Page
    {
        public BackTestAnalyzePage()
        {
            this.InitializeComponent();
            Loaded += OnLoaded;
            Unloaded += OnUnloaded;
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            ViewModel.ChartDataChanged -= ProfitLossDataOnCollectionChanged;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            ResizeChart(PnLChart, ViewModel.ProfitLossData.Count * 60);
            ViewModel.ChartDataChanged += ProfitLossDataOnCollectionChanged;
        }

        private void ProfitLossDataOnCollectionChanged(object sender, int e)
        {
            ResizeChart(PnLChart, e * 60);
        }

        private void ResizeChart(Chart chart, int suggestedWidth)
        {
            var edgePanel = chart.GetFirstDescendantOfType<EdgePanel>();

            if (edgePanel != null)
            {
                var width = chart.ActualWidth > suggestedWidth ? chart.ActualWidth : suggestedWidth;
                edgePanel.Width = width;
            }
        }

        private BackTestAnalyzeViewModel ViewModel
        {
            get { return DataContext as BackTestAnalyzeViewModel; }
        }
    }
}
