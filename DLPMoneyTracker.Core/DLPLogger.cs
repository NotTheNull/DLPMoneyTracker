using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLPMoneyTracker.Core;
public class DLPLogger(IDLPConfig config) : ILogger
{
    private readonly string LogFolderPath = Path.Combine(config.JSONFilePath, "Logs");
    private readonly string LogName = string.Format("Log_{0:yyyy_MM_dd}.txt", DateTime.Today);
    private string LogFilePath => Path.Combine(LogFolderPath, LogName);

    public LogLevel LoggingLevel { get; set; } = LogLevel.Warning;

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull => throw new NotImplementedException();
    public bool IsEnabled(LogLevel logLevel) => logLevel > this.LoggingLevel;
    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        if (!Directory.Exists(LogFolderPath)) Directory.CreateDirectory(LogFolderPath);

        string message = $"{Environment.NewLine}[{DateTime.Now}] {logLevel}: {formatter(state, exception)}";

        File.AppendAllText(LogFilePath, message);
    }
}
