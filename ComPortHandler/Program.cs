using ComPortHandler.Options.Logger.InfoLogger;
using ComPortHandler.Services;
using ComPortHandler.Services.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;


var s_cts = new CancellationTokenSource();
var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddSingleton<InfluxDbService>();
        services.AddSingleton<ComPortListenerService>();
        services.AddSingleton<IDataWorker, DataWorker>();
    })
    .ConfigureLogging(loggingBuilder =>
    {
        loggingBuilder.AddFile(o => o.RootPath = o.RootPath = AppContext.BaseDirectory);
        loggingBuilder.AddFile<InfoFileLoggerProvider>(configure: o => o.RootPath = AppContext.BaseDirectory);
    })
    .Build();

var logger = host.Services.GetRequiredService<ILogger<Program>>();
var comListener = host.Services.GetRequiredService<ComPortListenerService>();

try
{
    var cancelTask = Task.Run(() =>
    {
        while (Console.ReadKey().Key != ConsoleKey.Enter)
        {
            Console.WriteLine("Press the ENTER key to finish...");
        }

        Console.WriteLine("\nENTER key pressed: cancelling downloads.\n");
        s_cts.Cancel();
    });
    var listeningTask = comListener.OpenPortAndStartListenAsync();

    await Task.WhenAny(
        cancelTask,
        listeningTask
    );

    await comListener.ClosingPreparationAsync();
    comListener.ClosePort();
}
catch (Exception ex)
{
    logger.LogError(ex.Message);
}





