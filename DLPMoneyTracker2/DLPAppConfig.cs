using DLPMoneyTracker.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

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

        public string SQLConnectionString
        {
            get
            {
                return App.Config.GetConnectionString("sql") ?? string.Empty;
            }
        }
    }
}
