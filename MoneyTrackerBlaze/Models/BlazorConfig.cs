using DLPMoneyTracker.Core;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyTrackerBlaze.Models
{
    // NOTE: Blazor MAUI apps cannot access external files so any changes here would require a recompile
    public class BlazorConfig : IDLPConfig
    {
        const string SQL_SERVER = "server=DLP-HOME-PC\\SQLEXPRESS; database=MoneyTracker; Trusted_Connection=True; TrustServerCertificate=True";
        const string MYSQL = "server=localhost;database=fuzzyfu_dev;user=dlpeery;password=SS6HJT8m9zXccBtjyEbS";
        const string JSON_PATH = "D:\\Programs\\DLP Money Tracker";       

        public DLPDataSource DataSource { get { return DLPDataSource.Database; } }

        public string DBConnectionString { get { return SQL_SERVER; } }

        public string JSONFilePath { get { return JSON_PATH; } }
    }
}

