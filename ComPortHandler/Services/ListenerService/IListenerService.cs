using System.IO.Ports;

namespace ComPortHandler.Services.ListenerService
{
    public interface IListenerService
    {
        void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e);
        Task OpenPortAndStartListenAsync();
        void ClosingPreparationAsync();
        void ClosePort();
    }
}
