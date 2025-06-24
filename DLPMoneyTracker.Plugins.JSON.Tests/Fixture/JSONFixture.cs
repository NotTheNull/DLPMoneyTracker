using DLPMoneyTracker.BusinessLogic.AdapterInterfaces;
using DLPMoneyTracker.BusinessLogic.PluginInterfaces;
using DLPMoneyTracker.Core;
using DLPMoneyTracker.Plugins.JSON.Repositories;
using DLPMoneyTracker.TestModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DLPMoneyTracker.Plugins.JSON.Models;
using DLPMoneyTracker.Plugins.JSON.Adapters;

namespace DLPMoneyTracker.Plugins.JSON.Tests.Fixture
{
    internal class JSONFixture : IDisposable
    {

        private readonly IServiceProvider _serviceProvider;
        public JSONFixture()
        {
            var host = Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {
                    services.AddSingleton<IDLPConfig, TestJSONDLPConfig>();
                    services.AddSingleton<ILedgerAccountRepository, JSONLedgerAccountRepository>();
                    services.AddTransient<ISourceToJournalAccountAdapter<JournalAccountJSON>, JSONSourceToJournalAccountAdapter>();
                })
                .Build();
            _serviceProvider = host.Services;
                
        }


        public T GetService<T>() where T : class
        {
            return _serviceProvider.GetRequiredService<T>();
        }


        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
