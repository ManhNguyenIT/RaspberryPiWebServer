namespace RaspberryPiWebServer.Hubs
{
    public interface IMonitorHub
    {
        Task Monitor(string message);
    }
}
