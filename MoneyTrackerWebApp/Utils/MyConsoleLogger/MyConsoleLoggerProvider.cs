using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging.Configuration;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;

namespace MoneyTrackerWebApp.Utils.MyConsoleLogger
{
    // NOTE: Code copied from [https://github.com/dotnet/aspnetcore/issues/20504]

    [ProviderAlias("MyConsole")]
    internal class MyConsoleLoggerProvider : ILoggerProvider
    {

        private readonly IDisposable? _onChangeToken;
        private readonly ConcurrentDictionary<string, MyConsoleLogger> _listLoggers = new ConcurrentDictionary<string, MyConsoleLogger>(StringComparer.OrdinalIgnoreCase);
        private MyConsoleConfiguration _currentConfig;

        public MyConsoleLoggerProvider(IOptionsMonitor<MyConsoleConfiguration> config)
        {
            _currentConfig = config.CurrentValue;
            _onChangeToken = config.OnChange(updatedConfig => _currentConfig = updatedConfig);
        }

        public ILogger CreateLogger(string categoryName)
        {
            return _listLoggers.GetOrAdd(categoryName, name => new MyConsoleLogger(name, _currentConfig));
        }

        public void Dispose()
        {
            _listLoggers.Clear();
            _onChangeToken?.Dispose();
        }
    }


    internal class MyConsoleConfiguration
    {
        public LogLevel Default { get; set; }
    }

    internal static class MyConsoleLoggerBuilder
    {
        public static ILoggingBuilder AddMyConsoleLogger(this ILoggingBuilder builder)
        {
            builder.AddConfiguration();

            builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<ILoggerProvider, MyConsoleLoggerProvider>());
            LoggerProviderOptions.RegisterProviderOptions<MyConsoleConfiguration, MyConsoleLoggerProvider>(builder.Services);

            return builder;
        }

        public static ILoggingBuilder AddMyConsoleLogger(this ILoggingBuilder builder, Action<MyConsoleConfiguration> configure)
        {
            builder.AddMyConsoleLogger();
            builder.Services.Configure(configure);

            return builder;
        }

    }

    /*
    internal class MyConsoleLoggerProvider : ILoggerProvider
    {
        private static readonly Func<string, LogLevel, bool> DEFAULT_FILTER = (cat, level) => true;

        private readonly Func<string, LogLevel, bool> _filter;
        private ConcurrentDictionary<string, MyConsoleLogger> _listLoggers = new ConcurrentDictionary<string, MyConsoleLogger>();

        public MyConsoleLoggerProvider(Func<string, LogLevel, bool> filter)
        {
            _filter = filter ?? throw new ArgumentNullException(nameof(filter));
        }

        public Func<string, LogLevel, bool> Filter { get { return _filter ?? DEFAULT_FILTER; } }


        public ILogger CreateLogger(string categoryName)
        {
            if (string.IsNullOrWhiteSpace(categoryName)) throw new ArgumentNullException(nameof(categoryName));

            return _listLoggers.GetOrAdd(categoryName, new MyConsoleLogger(categoryName, this.Filter));
        }


        public void Dispose()
        {
            _listLoggers?.Clear();
        }
    }
    */
}
