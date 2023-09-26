using System.Windows.Controls;

namespace DLPMoneyTracker2.Main.YTD
{
    /// <summary>
    /// Interaction logic for YearToDateUI.xaml
    /// </summary>
    public partial class YearToDateUI : UserControl
    {
        private readonly YearToDateVM _viewModel;

        public YearToDateUI(YearToDateVM vm)
        {
            InitializeComponent();

            _viewModel = vm;
            this.DataContext = _viewModel;
        }
    }
}