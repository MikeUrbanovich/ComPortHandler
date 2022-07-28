using InfluxDB.Client;
using InfluxDB.Client.Api.Domain;
using InfluxDB.Client.Writes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ComPortHandler.Services.DbService
{
    public class InfluxDbService: IInfluxDbService
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private readonly WriteApiAsync _writeAsync;
        private readonly string _measurementName;
        private readonly string _bucketName;
        private readonly string _organizationName;

        public InfluxDbService(ILogger<InfluxDbService> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;

            var client = InfluxDBClientFactory.Create(
                _configuration.GetValue<string>("InfluxDB:Host"),
                _configuration.GetValue<string>("InfluxDB:Token")
            );
            _writeAsync = client.GetWriteApiAsync();
            _measurementName = _configuration.GetValue<string>("InfluxDB:Measurement");
            _bucketName = _configuration.GetValue<string>("InfluxDB:BucketName");
            _organizationName = _configuration.GetValue<string>("InfluxDB:OrganizationName");
        }

        public async Task? WriteAsync(string portName, string data)
        {
            _logger.LogDebug($"Write data {data} to DB");
            //_logger.LogInformation($"Write data {data} to DB");

            //await Task.Delay(5000);

            var point = PointData
                .Measurement(_measurementName)
                .Tag("PortName", portName)
                .Field("PortData", data)
                .Timestamp(DateTime.UtcNow, WritePrecision.S);

            try
            {
                await _writeAsync.WritePointAsync(point, _bucketName, _organizationName);
            }
            catch (Exception e)
            {
                throw new Exception($"Db task exception: {e.Message}");
            }

            _logger.LogDebug("data was written");
            //_logger.LogInformation("data was written");
        }
    }
}
