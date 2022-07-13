using ComPortHandler.Services;
using ComPortHandler.Services.Worker;
using InfluxDB.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;


var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddSingleton<InfluxDbService>();
        services.AddSingleton<InfluxDBClient>();
        services.AddSingleton<ComPortListenerService>();
        services.AddSingleton<IDataWorker, DataWorker>();
    })
    .ConfigureLogging(loggingBuilder =>
    {
        loggingBuilder.AddFile($"{Directory.GetCurrentDirectory()}\\Logs\\log.txt");
    })
    .Build();

var logger = host.Services.GetRequiredService<ILogger<Program>>();
var comListener = host.Services.GetRequiredService<ComPortListenerService>();

try
{
    comListener.OpenPortAndStartListen();
    Console.ReadKey();

    await comListener.ClosingPreparation();
    comListener.ClosePort();
}
catch (Exception ex)
{
    logger.LogError(ex.Message);
}





