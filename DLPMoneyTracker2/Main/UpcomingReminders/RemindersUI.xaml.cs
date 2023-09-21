using System.Windows.Controls;

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