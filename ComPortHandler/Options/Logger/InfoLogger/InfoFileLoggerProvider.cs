using Karambolo.Extensions.Logging.File;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ComPortHandler.Options.Logger.InfoLogger
{
    [ProviderAlias("InfoFile")]
    internal class InfoFileLoggerProvider : FileLoggerProvider
    {
        public InfoFileLoggerProvider(
            FileLoggerContext context,
            IOptionsMonitor<FileLoggerOptions> options,
            string optionsName
            ) : base(context, options, optionsName) { }

        protected override FileLogger CreateLoggerCore(string categoryName)
        {
            return new InfoFileLogger(categoryName, Processor, Settings, GetScopeProvider(), Context.GetTimestamp);
        }
    }
}
