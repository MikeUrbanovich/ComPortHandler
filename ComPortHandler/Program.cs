using ComPortHandler.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using IHost host = Host.CreateDefaultBuilder()
    .ConfigureServices(services =>
    {
        services.AddSingleton<InfluxDBService>();
    })
    .Build();

var db = host.Services.GetRequiredService<InfluxDBService>();

db.Write("");

var config = host.Services.GetRequiredService<IConfiguration>();

int keyOneValue = config.GetValue<int>("KeyOne");

Console.WriteLine(keyOneValue);
Console.ReadKey();


