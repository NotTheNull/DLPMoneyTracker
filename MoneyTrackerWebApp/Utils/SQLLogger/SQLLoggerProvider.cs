using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Configuration;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;

namespace MoneyTrackerWebApp.Utils.SQLLogger
{
    internal class SQLLoggerProvider : ILoggerProvider
    {
        private readonly IDisposable? _onConfigChangeToken;
        private readonly ConcurrentDictionary<string, SQLLogger> _listLoggers = new ConcurrentDictionary<string, SQLLogger>();
        private SQLLoggerConfiguration _currentConfig;

        public SQLLoggerProvider(IOptionsMonitor<SQLLoggerConfiguration> config)
        {
            _currentConfig = config.CurrentValue;
            _onConfigChangeToken = config.OnChange(updatedConfig => _currentConfig = updatedConfig);
        }

        public ILogger CreateLogger(string categoryName)
        {
            return _listLoggers.GetOrAdd(categoryName, name => new SQLLogger(name, _currentConfig));
        }

        public void Dispose()
        {
            _listLoggers.Clear();
            _onConfigChangeToken?.Dispose();
        }
    }

    internal class SQLLoggerConfiguration
    {
        public LogLevel Default { get; set; }
        public string ConnectionString { get; set; }
    }

    internal static class SQLLoggerBuilder
    {
        public static ILoggingBuilder AddSQLLogger(this ILoggingBuilder builder)
        {
            builder.AddConfiguration();

            builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<ILoggerProvider, SQLLoggerProvider>());
            LoggerProviderOptions.RegisterProviderOptions<SQLLoggerConfiguration, SQLLoggerProvider>(builder.Services);

            return builder;
        }

        public static ILoggingBuilder AddSQLLogger(this ILoggingBuilder builder, Action<SQLLoggerConfiguration> configure)
        {
            builder.AddSQLLogger();
            builder.Services.Configure(configure);

            return builder;
        }


        public static LogLevel ToLogLevel(this string level)
        {
            switch (level.ToLower())
            {
                case "critical": return LogLevel.Critical;
                case "error": return LogLevel.Error;
                case "warning": return LogLevel.Warning;
                case "information": return LogLevel.Information;
                case "debug": return LogLevel.Debug;
                case "trace": return LogLevel.Trace;
                default: return LogLevel.None;
            }
        }
    }
}
