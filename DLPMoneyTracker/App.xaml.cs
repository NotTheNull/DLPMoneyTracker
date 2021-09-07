using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace DLPMoneyTracker
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            UICore.Init();
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            var mainWindow = UICore.DependencyHost.GetService<MainWindow>();
            mainWindow.Show();
        }
    }

    // TODO: Reports aren't doing it; Replace them with a clone of the Budget Planner UI that has Month/Year filters
}