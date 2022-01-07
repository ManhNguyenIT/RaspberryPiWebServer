using Microsoft.AspNetCore.SignalR;
using RaspberryPiWebServer.Services;

namespace RaspberryPiWebServer.Hubs
{
    public class MonitorHub : Hub<IMonitorHub>
    {
        private readonly MonitorService _service;

        public MonitorHub(MonitorService service)
        {
            _service = service;
        }
        public async Task ClickAsync(bool value)
        {
            var settings = _service.Read();
            Models.Item item = settings.Outputs.First();
            item.Value = !item.Value;
            _service.Write(item);
            await Clients.All.Monitor(Newtonsoft.Json.JsonConvert.SerializeObject(_service.Read()));
        }
    }
}
