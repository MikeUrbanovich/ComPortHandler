using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ComPortHandler.Services.Worker
{
    public class DataWorker : IDataWorker
    {
        private readonly ILogger<DataWorker> _logger;
        private readonly IConfiguration _configuration;
        private readonly InfluxDbService _influxDbService;
        private Queue<string> _buffer = new ();
        private Task DbTask { get; set; }

        public DataWorker(
            ILogger<DataWorker> logger,
            InfluxDbService influxDbService,
            IConfiguration configuration)
        {
            _influxDbService = influxDbService;
            _configuration = configuration;
            _logger = logger;
        }

        public async void StartDataProcessing()
        {
            _logger.LogInformation("Start data processing");

            var portName = _configuration.GetValue<string>("PortSettings:PortName");

            while (true)
            {
                if (!CheckDataInBuffer())
                {
                    continue;
                }

                DbTask = _influxDbService.WriteAsync(portName, _buffer.Dequeue());
                await DbTask;
            }
        }

        public async Task StopDataProcessing()
        {
            await DbTask;
            _logger.LogInformation("Finish data processing");
        }

        public void AddDataToBuffer(string data)
        {
            if (Validate(data))
            {
                _buffer.Enqueue(data);
            }
        }

        public bool CheckDataInBuffer() => _buffer.Count != 0;

        private bool Validate(string data)
        {
            const string pattern = @"^\d{4}:\d{2}:\d{2}\s\d{7}$";

            if (string.IsNullOrEmpty(data))
            {
                _logger.LogWarning($"Data {data} from port are incorrect.");
                return false;
            }
            if (!Regex.IsMatch(data, pattern, RegexOptions.IgnoreCase))
            {
                _logger.LogWarning($"Data {data} from port are incorrect.");
                return false;
            }

            return true;
        }
    }
}
