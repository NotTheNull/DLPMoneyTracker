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

namespace DLPMoneyTracker2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Exit()
        {
            // Possible that I'll need to ask before exit at some point
            // Also making this a separate method in case I have multiple events to trigger it
            System.Windows.Application.Current.Shutdown();
        }

        private void MenuExit_Click(object sender, RoutedEventArgs e)
        {
            this.Exit();
        }
    }
}
