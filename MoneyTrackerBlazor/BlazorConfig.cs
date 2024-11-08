using DLPMoneyTracker.Core;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyTrackerBlazor
{
    public class BlazorConfig : IDLPConfig
    {
        
        public BlazorConfig(IConfiguration config)
        {
            this.JSONFilePath = config.GetValue<string>("ConnectionStrings:json_path");

            string connName = config.GetValue<string>("AppSettings:connName");
            this.DBConnectionString = config.GetValue<string>($"ConnectionStrings:{connName}");
            this.DataSource = config.GetValue<string>("AppSettings:source").ToDataSource();
        }

        public DLPDataSource DataSource { get; set; }

        public string DBConnectionString { get; set; }

        public string JSONFilePath { get; set; }
    }
}
