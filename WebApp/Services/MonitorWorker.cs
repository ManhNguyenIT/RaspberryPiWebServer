using Microsoft.AspNetCore.SignalR;
using WebApp.Hubs;
using System.Device.Gpio;

namespace WebApp.Services
{
    public class MonitorWorker : BackgroundService
    {
        private readonly IHubContext<MonitorHub, IMonitorHub> _hub;
        private readonly MonitorService _service;

        public MonitorWorker(IHubContext<MonitorHub, IMonitorHub> hub,
                            MonitorService service)
        {
            _hub = hub;
            _service = service;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await _hub.Clients.All.Monitor(Newtonsoft.Json.JsonConvert.SerializeObject(_service.Read()));
                //Run the background service for every 10 seconds  
                await Task.Delay(500, stoppingToken);
            }
        }
    }
}