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

}