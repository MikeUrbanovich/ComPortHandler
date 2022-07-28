using ComPortHandler.Options.Logger.InfoLogger;
using ComPortHandler.Services.DbService;
using ComPortHandler.Services.ListenerService;
using ComPortHandler.Services.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ComPortHandler
{
    internal class Startup
    {
        private IConfiguration Configuration;

        public Startup()
        {
            Configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.AddFile(o => o.RootPath = AppContext.BaseDirectory);
                loggingBuilder.AddFile<InfoFileLoggerProvider>(configure: o => o.RootPath = AppContext.BaseDirectory);
            });

            services.AddSingleton(Configuration);

            services
                .AddTransient<IInfluxDbService, InfluxDbService>()
                .AddTransient<IListenerService, ComPortListenerService>()
                .AddTransient<IDataWorker, DataWorker>()
                .AddLogging(c => c.AddConsole());
        }
    }
}
