using System.IO.Ports;
using ComPortHandler.Services.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ComPortHandler.Services
{
    internal class ComPortListenerService
    {
        private readonly SerialPort _serialPort;
        private readonly ILogger<ComPortListenerService> _logger;
        private readonly IDataWorker _dataWorker;

        public ComPortListenerService(
            IConfiguration configuration,
            ILogger<ComPortListenerService> logger,
            IDataWorker dataWorker)
        {
            _logger = logger;
            _dataWorker = dataWorker;

            _serialPort = new SerialPort
            {
                PortName = configuration.GetValue<string>("PortSettings:PortName")
            };
            _serialPort.DataReceived += DataReceivedHandler;
        }


        private void DataReceivedHandler(
            object sender,
            SerialDataReceivedEventArgs e)
        {
            var sp = (SerialPort)sender;

            var data = sp.ReadExisting()
                .Replace("\n", "").
                Replace("\r", "");

            _logger.LogInformation($"Data from Port {_serialPort.PortName}: {data}");

            _dataWorker.AddDataToBuffer(data);
        }

        public void OpenPortAndStartListen()
        {
            if (_serialPort.IsOpen)
            {
                _serialPort.Close();
            }

            _serialPort.Open();
            _logger.LogInformation($"Port {_serialPort.PortName} is open. Start listening.");
            _logger.LogInformation("Press any key to finish...");

            _dataWorker.StartDataProcessing();
        }

        public async Task ClosingPreparation()
        {
            _serialPort.DataReceived -= DataReceivedHandler;
            _logger.LogInformation("Stop listening.");

            while (_dataWorker.CheckDataInBuffer())
            {
                Thread.Sleep(2000);
                _logger.LogInformation("Wait handling all data...");
            }

            await _dataWorker.StopDataProcessing();
        }

        public void ClosePort()
        {
            if (_serialPort.IsOpen)
            {
                _serialPort.Close();
            }

            _logger.LogInformation($"Port {_serialPort.PortName} is close.");
        }
    }
}
