﻿using DLPMoneyTracker.Data;
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
            UICore.Init();
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            // Handle conversions
#pragma warning disable CS0612 // Type or member is obsolete
            var config = UICore.DependencyHost.GetService<ITrackerConfig>();
            if (config?.AccountsList?.Any() == true || config?.CategoryList?.Any() == true)
            {
                config.Convert();
            }

            var ledger = UICore.DependencyHost.GetService<ILedger>();
            if (ledger?.TransactionList.Any() == true)
            {
                var journal = UICore.DependencyHost.GetService<IJournal>();
                journal?.Convert(ledger);
            }
#pragma warning restore CS0612 // Type or member is obsolete


            var mainWindow = UICore.DependencyHost.GetService<MainWindow>();
            mainWindow?.Show();
        }
    }
    
}