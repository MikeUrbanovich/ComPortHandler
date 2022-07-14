namespace ComPortHandler.Services.Worker
{
    public interface IDataWorker
    {
        void AddDataToBuffer(string data);
        bool CheckDataInBuffer();
        Task StartDataProcessingAsync();
        Task StopDataProcessingAsync();

    }
}
