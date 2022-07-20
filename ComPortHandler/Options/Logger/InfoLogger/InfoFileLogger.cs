using Karambolo.Extensions.Logging.File;
using Microsoft.Extensions.Logging;

namespace ComPortHandler.Options.Logger.InfoLogger
{
    internal class InfoFileLogger : FileLogger
    {
        public InfoFileLogger(
            string categoryName,
            IFileLoggerProcessor processor,
            IFileLoggerSettings settings,
            IExternalScopeProvider? scopeProvider = null,
            Func<DateTimeOffset>? timestampGetter = null
            ) : base(categoryName, processor, settings, scopeProvider, timestampGetter) { }

        public override bool IsEnabled(LogLevel logLevel)
        {
            return
                logLevel <= LogLevel.Information && 
                base.IsEnabled(logLevel);
        }
    }
}
