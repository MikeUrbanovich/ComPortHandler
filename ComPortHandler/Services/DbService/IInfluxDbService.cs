namespace ComPortHandler.Services.DbService
{
    public interface IInfluxDbService
    {
        Task? WriteAsync(string portName, string data);
    }
}
