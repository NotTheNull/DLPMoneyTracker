using DLPMoneyTracker.Core;

namespace MoneyTrackerWebApp.Models.Core
{
    public class BlazorConfig : IDLPConfig
    {

        public BlazorConfig(IConfiguration config)
        {
            this.DataSource = config.GetValue<string>("AppSettings:source")?.ToDataSource() ?? DLPDataSource.NotSet;
            
            string connName = config.GetValue<string>("AppSettings:connName") ?? string.Empty;
            this.DBConnectionString = config.GetConnectionString(connName) ?? string.Empty;

            this.JSONFilePath = config.GetConnectionString("json_path") ?? Directory.GetCurrentDirectory();
        }


        public DLPDataSource DataSource { get; set; }

        public string DBConnectionString { get; set; }

        public string JSONFilePath { get; set; }
    }
}
