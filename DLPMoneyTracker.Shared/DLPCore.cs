using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;

namespace DLPMoneyTracker.Shared
{
    public static class DLPCore
    {
        public const string CONFIG_PATH = @"D:\Program Files\DLP Money Tracker\Config\";

        public static IServiceProvider DependencyHost { get; set; }


        public static void Init()
        {
            if (!Directory.Exists(CONFIG_PATH))
            {
                Directory.CreateDirectory(CONFIG_PATH);
            }


            ServiceCollection services = new ServiceCollection();
            ConfigureServices(services);
            DependencyHost = services.BuildServiceProvider();
        }

        private static void ConfigureServices(ServiceCollection services)
        {
            services.AddScoped<AddEditMoneyAccount>();
            services.AddSingleton<TrackerConfig>();
            services.AddSingleton<MainWindow>();
        }
    }
}
