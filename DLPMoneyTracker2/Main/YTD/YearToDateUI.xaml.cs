using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
