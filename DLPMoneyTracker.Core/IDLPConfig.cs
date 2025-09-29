using DLPMoneyTracker.Core.Models;

namespace DLPMoneyTracker.Core
{
    public enum DLPDataSource
    {
        NotSet,
        JSON,
        Database
    }

    public interface IDLPConfig
    {
        DLPDataSource DataSource { get; }
        string DBConnectionString { get; }
        string JSONFilePath { get; }
        PayPeriod Period { get; }
    }
}