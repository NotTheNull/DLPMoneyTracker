using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using DLPMoneyTracker.Data;
using DLPMoneyTracker.DataEntry.AddEditMoneyAccount;

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
