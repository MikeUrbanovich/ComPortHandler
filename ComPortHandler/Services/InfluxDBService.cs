using InfluxDB.Client;
using InfluxDB.Client.Api.Domain;
using InfluxDB.Client.Writes;
using Microsoft.Extensions.Configuration;

namespace ComPortHandler.Services
{
    public class InfluxDBService
    {
        private readonly string _token;
        private readonly string _host;

        public InfluxDBService(IConfiguration configuration)
        {
            _token = configuration.GetValue<string>("InfluxDB:Token");
            _host = configuration.GetValue<string>("InfluxDB:Host");
        }

        public void Write(string data)
        {
            using var client = InfluxDBClientFactory.Create(_host, _token);
            using var write = client.GetWriteApi();

            var point = PointData.Measurement("com_port_data")
                    .Tag("PortNumber", "testNumber")
                    .Field("PortData", 55D)
                    .Timestamp(DateTime.UtcNow, WritePrecision.Ns);

            write.WritePoint(point, "new_test_bucket", "murbCompany");

            var tmp = "";
        }
    }
}
