using System.Windows.Controls;

namespace DLPMoneyTracker2.Main.TransactionList
{
    /// <summary>
    /// Interaction logic for TransactionDetail.xaml
    /// </summary>
    public partial class TransactionDetail : UserControl
    {
        public TransactionDetail(TransactionDetailVM viewModel)
        {
            InitializeComponent();
            this.DataContext = viewModel;
        }
    }
}