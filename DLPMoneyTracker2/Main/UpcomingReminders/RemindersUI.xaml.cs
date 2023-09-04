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

namespace DLPMoneyTracker2.Main.UpcomingReminders
{
    /// <summary>
    /// Interaction logic for RemindersUI.xaml
    /// </summary>
    public partial class RemindersUI : UserControl
    {
        public RemindersUI(RemindersVM vm)
        {
            InitializeComponent();
            this.DataContext = vm;
            vm.Load();
        }
    }
}
