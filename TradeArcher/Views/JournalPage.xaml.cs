using TradeArcher.ViewModels;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace TradeArcher.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class JournalPage : Page
    {
        public JournalPage()
        {
            this.InitializeComponent();
        }

        private JournalViewModel ViewModel
        {
            get { return DataContext as JournalViewModel; }
        }
    }
}
