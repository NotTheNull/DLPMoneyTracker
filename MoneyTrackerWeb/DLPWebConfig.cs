using DLPMoneyTracker.Core;

namespace MoneyTrackerWeb
{
    public class DLPWebConfig : IDLPConfig
    {
        public DLPWebConfig(IConfiguration config)
        {
            string source = config.GetValue<string>("DataSource");
            this.DBConnectionString = config.GetValue<string>($"ConnectionStrings:{source}");
        }

        // Will NOT use JSON because the files will be out of reach (they're with the Desktop app)
        public DLPDataSource DataSource { get { return DLPDataSource.Database; } }

        public string DBConnectionString { get; set; }
    }
}
