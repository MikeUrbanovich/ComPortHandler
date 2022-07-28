using System.Text.RegularExpressions;
using ComPortHandler.Services.DbService;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ComPortHandler.Services.Worker
{
    public class DataWorker : IDataWorker
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private readonly IInfluxDbService _influxDbService;
        private Queue<string> _buffer = new ();
        private Task? DbTask { get; set; } = null;

        public DataWorker(
            ILogger<DataWorker> logger,
            IInfluxDbService influxDbService,
            IConfiguration configuration)
        {
            _influxDbService = influxDbService;
            _configuration = configuration;
            _logger = logger;
        }

        //Check data in buffer.
        //If data exists, they will be deleted from buffer and passed like argument for WriteAsync db task.
        public async Task StartDataProcessingAsync()
        {
            _logger.LogInformation("Start data processing");

            var portName = _configuration.GetValue<string>("PortSettings:PortName");

            while (true)
            {
                if (_buffer.Count == 0)
                {
                    continue;
                }

                DbTask = _influxDbService.WriteAsync(portName, _buffer.Dequeue());
                try
                {
                    await DbTask;
                }
                catch (Exception e)
                {
                    throw new Exception(e.Message);
                }
            }
        }

        //Add valid data into buffer
        public void AddDataToBuffer(string data)
        {
            if (Validate(data))
            {
                _buffer.Enqueue(data);
            }
        }

        public bool CheckDataInBuffer()
        {
            if (_buffer.Count != 0)
                return true;

            return _buffer.Count switch
            {
                0 when DbTask == null => false,
                0 when DbTask.IsCompleted => false,
                _ => true
            };
        }

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
