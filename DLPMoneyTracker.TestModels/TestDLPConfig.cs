using DLPMoneyTracker.Core;

namespace DLPMoneyTracker.TestModels
{
    public class TestDLPConfig : IDLPConfig
    {
        public DLPDataSource DataSource { get; set; } = DLPDataSource.NotSet;

        public string DBConnectionString { get; set; } = string.Empty;

        public string JSONFilePath { get; set; } = string.Empty;
    }
}
