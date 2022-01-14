namespace WebApp.Hubs
{
    public interface IMonitorHub
    {
        Task Monitor(string message);
    }
}
