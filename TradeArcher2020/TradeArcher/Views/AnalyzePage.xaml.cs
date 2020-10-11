using Windows.UI.Xaml.Controls;
using TradeArcher.ViewModels;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace TradeArcher.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AnalyzePage : Page
    {
        public AnalyzePage()
        {
            this.InitializeComponent();
        }

        private AnalyzeViewModel ViewModel
        {
            get { return DataContext as AnalyzeViewModel; }
        }
    }
}
