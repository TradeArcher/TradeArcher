using Windows.UI.Xaml.Controls;
using Caliburn.Micro;
using TradeArcher.ViewModels;
using WinUI = Microsoft.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace TradeArcher.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class BrokersPage : Page
    {
        public BrokersPage()
        {
            this.InitializeComponent();
        }

        private BrokersViewModel ViewModel
        {
            get { return DataContext as BrokersViewModel; }
        }
    }
}
