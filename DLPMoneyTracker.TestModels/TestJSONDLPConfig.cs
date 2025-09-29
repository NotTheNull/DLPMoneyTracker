using DLPMoneyTracker.Core;
using DLPMoneyTracker.Core.Models;

namespace DLPMoneyTracker.TestModels;

public class TestJSONDLPConfig : IDLPConfig
{
    public DLPDataSource DataSource => DLPDataSource.JSON;

    public string DBConnectionString => string.Empty;

    private const string TMP_PATH = "C:\\Users\\crc\\AppData\\Local\\Temp";
    public string JSONFilePath => Path.Combine(TMP_PATH, "MoneyTrackerTest");

    public PayPeriod Period => new() { 
        NumberOfDays = 14,
        StartDate = DateTime.Today
    };
}
