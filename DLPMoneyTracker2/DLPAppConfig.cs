using DLPMoneyTracker.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace DLPMoneyTracker2
{
    public class DLPAppConfig : IDLPConfig
    {
        public DLPDataSource DataSource
        {
            get
            {
                return App.Config["AppSettings:source"]?.ToDataSource() ?? DLPDataSource.NotSet;
            }
        }

        public string DBConnectionString
        {
            get
            {
                string connName = App.Config["AppSettings:connName"]?.ToString() ?? string.Empty;
                
                return App.Config.GetConnectionString(connName) ?? string.Empty;
            }
        }

        public string JSONFilePath { get { return Directory.GetCurrentDirectory(); } }
    }
}
