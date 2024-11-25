using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Collections.Concurrent;

namespace MoneyTrackerWebApp.Utils
{
    // NOTE: Code copied from [https://github.com/dotnet/aspnetcore/issues/20504]

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

    internal static class MyConsoleLoggerConfiguration
    {
        public static ILoggingBuilder AddMyConsoleLogger(this ILoggingBuilder builder)
        {
            builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<ILoggerProvider, MyConsoleLoggerProvider>());

            // HACK: Replace the Blazor built-in logger (which fails to log to the console) with this provider
            builder.Services.Add(ServiceDescriptor.Singleton(typeof(ILogger<>), typeof(MyConsoleLogger<>)));

            return builder;
        }

    }
}
