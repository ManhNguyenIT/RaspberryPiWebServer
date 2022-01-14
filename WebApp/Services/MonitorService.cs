using Microsoft.Extensions.Options;
using WebApp.Models;
using System.Device.Gpio;

namespace WebApp.Services
{
    public class MonitorService
    {
        private readonly Settings _settings;
        private readonly ILogger<MonitorService> _logger;
        private readonly GpioController _controller;
        public MonitorService(IOptions<Settings> settings, ILogger<MonitorService> logger)
        {
            _controller = new GpioController();
            _settings = settings?.Value;
            _logger = logger;
            if (_settings != null)
            {
                try
                {
                    for (int i = 0; i < _settings.Inputs.Length; i++)
                    {
                        Item input = _settings.Inputs[i];
                        _controller.OpenPin(input.Pin, PinMode.InputPullUp);
                        input.Value = true;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.InnerException?.Message ?? ex.Message);
                }
                try
                {
                    for (int i = 0; i < _settings.Outputs.Length; i++)
                    {
                        Item output = _settings.Outputs[i];
                        _controller.OpenPin(output.Pin, PinMode.Output);
                        _controller.Write(output.Pin, PinValue.High);
                        output.Value = true;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.InnerException?.Message ?? ex.Message);
                }
            }
        }

        public void Write(Item item)
        {
            if (_settings.Outputs.Any(i => i.Name == item.Name && i.Pin == item.Pin))
            {
                try
                {
                    _controller.Write(item.Pin, item.Value);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.InnerException?.Message ?? ex.Message);
                }
            }
        }

        public async Task WriteAsync(Item item, int Delay)
        {
            if (_settings.Outputs.Any(i => i.Name == item.Name && i.Pin == item.Pin))
            {
                try
                {
                    _controller.Write(item.Pin, item.Value);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.InnerException?.Message ?? ex.Message);
                }
                await Task.Delay(Delay);
                try
                {
                    _controller.Write(item.Pin, !item.Value);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.InnerException?.Message ?? ex.Message);
                }
            }
        }

        public Settings Read()
        {
            if (_settings == null)
            {
                return _settings;
            }

            foreach (var item in _settings.Inputs)
            {
                try
                {
                    item.Value = _controller.Read(item.Pin) == PinValue.High;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.InnerException?.Message ?? ex.Message);
                }
            }

            return _settings;
        }

    }
}