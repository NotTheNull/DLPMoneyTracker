using System.Text.Json;
using Microsoft.Data.SqlClient;

namespace MoneyTrackerWebApp.Utils.SQLLogger
{
    internal class SQLLogger<T> : SQLLogger, ILogger<T>
    {
        public SQLLogger(SQLLoggerConfiguration config) : base(typeof(T).FullName, config) { }

    }


    // Just use SQL Command to insert the data; no need to involve Entity Framework for this
    internal class SQLLogger : ILogger
    {
        private readonly string sourceName;
        private readonly SQLLoggerConfiguration config;

        public SQLLogger(string name, SQLLoggerConfiguration config)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(name);
            ArgumentNullException.ThrowIfNull(config);

            sourceName = name;
            this.config = config;
        }

        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => default;

        public bool IsEnabled(LogLevel logLevel)
        {
            return logLevel >= config.Default;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            if (!IsEnabled(logLevel)) return;

            string message = formatter != null ? formatter(state, exception) : state.ToString();
            string jsonError = "";
            if (exception != null)
            {
                jsonError = JsonSerializer.Serialize(exception);
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(config.ConnectionString))
                {
                    string query = $"INSERT INTO DebugLog (ModuleName, LogMessage, ErrorJSON) VALUES('{sourceName}', '{message}', '{jsonError}')";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.ExecuteNonQuery();
                }
            }
            catch
            {
                // Can't log it, so don't worry about it
            }

        }
    }
}
