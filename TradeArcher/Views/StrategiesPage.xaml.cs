using Windows.UI.Xaml.Controls;
using TradeArcher.ViewModels;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace TradeArcher.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class StrategiesPage : Page
    {
        public StrategiesPage()
        {
            this.InitializeComponent();
        }

        private StrategiesPageViewModel ViewModel
        {
            get { return DataContext as StrategiesPageViewModel; }
        }
    }
}
