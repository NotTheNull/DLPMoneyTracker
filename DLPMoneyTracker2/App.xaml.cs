using DLPMoneyTracker.Data;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace DLPMoneyTracker2
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            UICore.Init();

            // Handle conversions
#pragma warning disable CS0612 // Type or member is obsolete

            //if (MessageBox.Show("Do you want to convert?", "Initial Conversion", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            //{
            //    //var config = UICore.DependencyHost.GetService<ITrackerConfig>();
            //    //if (config?.AccountsList?.Any() == true || config?.CategoryList?.Any() == true)
            //    //{
            //    //    config.Convert();
            //    //}

            //    //var ledger = UICore.DependencyHost.GetService<ILedger>();
            //    //if (ledger?.TransactionList.Any() == true)
            //    //{
            //    //    var journal = UICore.DependencyHost.GetService<IJournal>();
            //    //    journal?.Convert(ledger);
            //    //}

            //    //var oldPlanner = UICore.DependencyHost.GetService<IMoneyPlanner>();
            //    //oldPlanner.LoadFromFile();
            //    //if (oldPlanner?.MoneyPlanList.Any() == true)
            //    //{
            //    //    var planner = UICore.DependencyHost.GetService<IJournalPlanner>();
            //    //    planner.Convert(oldPlanner);
            //    //}
            //}

#pragma warning restore CS0612 // Type or member is obsolete


            var mainWindow = UICore.DependencyHost.GetService<MainWindow>();
            mainWindow?.Show();
        }
    }

}
