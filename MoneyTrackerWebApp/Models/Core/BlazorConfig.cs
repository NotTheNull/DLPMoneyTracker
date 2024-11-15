using DLPMoneyTracker.Core;

namespace MoneyTrackerWebApp.Models.Core
{
    public class BlazorConfig : IDLPConfig
    {
        public DLPDataSource DataSource => throw new NotImplementedException();

        public string DBConnectionString => throw new NotImplementedException();

        public string JSONFilePath => throw new NotImplementedException();
    }
}
