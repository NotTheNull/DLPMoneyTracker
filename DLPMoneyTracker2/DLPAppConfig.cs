using DLPMoneyTracker.Core;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace DLPMoneyTracker2
{
    public class DLPAppConfig : IDLPConfig
    {
        public DLPDataSource DataSource => App.Config["AppSettings:source"]?.ToDataSource() ?? DLPDataSource.NotSet;

        public string DBConnectionString
        {
            get
            {
                string connName = App.Config["AppSettings:connName"]?.ToString() ?? string.Empty;

                return App.Config.GetConnectionString(connName) ?? string.Empty;
            }
        }

        public string JSONFilePath => App.Config.GetConnectionString("json_path") ?? Directory.GetCurrentDirectory();
    }
}