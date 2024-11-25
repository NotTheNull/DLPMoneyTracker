using System.ComponentModel;

namespace MoneyTrackerWebApp.Utils
{
    internal class MyConsoleLogger<T> : MyConsoleLogger, ILogger<T>
    {
        public MyConsoleLogger() : base(typeof(T).FullName, null) { }
        public MyConsoleLogger(Func<string, LogLevel, bool> filter) : base(typeof(T).FullName, filter) { }
    }


    internal class MyConsoleLogger : ILogger
    {
        public MyConsoleLogger(string name, Func<string, LogLevel, bool> filter) 
        {
            this.Name = name ?? throw new ArgumentNullException(nameof(name));
            this.Filter = filter ?? ((category, logLevel) => true);
        }

        public Func<string, LogLevel, bool> Filter { get; set; }
        public string Name { get; }

        public bool IsEnabled(LogLevel lvl)
        {
            return lvl != LogLevel.None && this.Filter(this.Name, lvl);
        }

        public IDisposable BeginScope<TState>(TState state) => null;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            if (!this.IsEnabled(logLevel)) return;

            string message = formatter != null ? formatter(state, exception) : state.ToString();
            Console.WriteLine($"[{this.Name}]: {message}");
            if(exception != null)
            {
                Console.Error.WriteLine(exception.ToString());
            }

        }
    }
}
