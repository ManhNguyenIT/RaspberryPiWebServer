using Microsoft.AspNetCore.SignalR;
using WebApp.Services;

namespace WebApp.Hubs
{
    public class MonitorHub : Hub<IMonitorHub>
    {
        private readonly MonitorService _service;

        public MonitorHub(MonitorService service)
        {
            _service = service;
        }
    }
}
