using InfluxDB.Client;
using InfluxDB.Client.Api.Domain;
using InfluxDB.Client.Writes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ComPortHandler.Services
{
    public class InfluxDbService
    {
        private readonly ILogger<InfluxDbService> _logger;
        private readonly WriteApiAsync _writeAsync;
        private readonly string _measurementName;
        private readonly string _bucketName;
        private readonly string _organizationName;

        public InfluxDbService(IConfiguration configuration, ILogger<InfluxDbService> logger)
        {
            _logger = logger;

            var client = InfluxDBClientFactory.Create(
                configuration.GetValue<string>("InfluxDB:Host"),
                configuration.GetValue<string>("InfluxDB:Token")
            );
            _writeAsync = client.GetWriteApiAsync();
            _measurementName = configuration.GetValue<string>("InfluxDB:Measurement");
            _bucketName = configuration.GetValue<string>("InfluxDB:BucketName");
            _organizationName = configuration.GetValue<string>("InfluxDB:OrganizationName");
        }

        public async Task WriteAsync(string portName, string data)
        {
            _logger.LogDebug($"Write data {data} to DB");

            await Task.Delay(2000);

            var point = PointData
                .Measurement(_measurementName)
                .Tag("PortName", portName)
                .Field("PortData", data)
                .Timestamp(DateTime.UtcNow, WritePrecision.S);

            await _writeAsync.WritePointAsync(point, _bucketName, _organizationName);
            _logger.LogDebug("data was write");
        }
    }
}
