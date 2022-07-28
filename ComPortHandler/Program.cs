using ComPortHandler;
using ComPortHandler.Services.ListenerService;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;


var startup = new Startup();
var services = new ServiceCollection();
var sCts = new CancellationTokenSource();

startup.ConfigureServices(services);
var serviceProvider = services.BuildServiceProvider();

var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
var comListener = serviceProvider.GetRequiredService<IListenerService>();

try
{
    var cancelTask = Task.Run(() =>
    {
        while (Console.ReadKey().Key != ConsoleKey.Enter)
        {
            logger.LogInformation("Press the ENTER key to finish...");
        }

        logger.LogInformation("\nENTER key pressed: cancelling downloads.\n");
        sCts.Cancel();
    });
    var listeningTask = comListener.OpenPortAndStartListenAsync();

    await Task.WhenAny(
        cancelTask,
        listeningTask
    );

    comListener.ClosingPreparationAsync();
    comListener.ClosePort();
}
catch (Exception ex)
{
    logger.LogError(ex.Message);
}