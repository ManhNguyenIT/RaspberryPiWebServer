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
            _controller = new GpioController(PinNumberingScheme.Board);
            _settings = settings?.Value;
            _logger = logger;

            _logger.LogInformation("*************INIT GPIO*************");
            if (_settings != null)
            {
                _logger.LogInformation("*************INIT INPUT GPIO*************");
                try
                {
                    for (int i = 0; i < _settings.Inputs.Length; i++)
                    {
                        Item input = _settings.Inputs[i];
                        if (_controller.IsPinModeSupported(input.Pin, PinMode.InputPullDown))
                        {
                            _controller.OpenPin(input.Pin, PinMode.InputPullDown);
                            input.Value = true;
                        }
                        else
                        {
                            throw new ArgumentException($"Pin {input.Pin} not support mode InputPullDown");
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.InnerException?.Message ?? ex.Message);
                }
                _logger.LogInformation("*************INIT OUTPUT GPIO*************");
                try
                {
                    //for (int i = 0; i < _settings.Outputs.Length; i++)
                    //{
                    //    Item output = _settings.Outputs[i];
                    //    if (_controller.IsPinModeSupported(output.Pin, PinMode.Output))
                    //    {
                    //        _controller.OpenPin(output.Pin, PinMode.Output);
                    //        _controller.Write(output.Pin, PinValue.Low);
                    //        output.Value = false;
                    //    }
                    //    else
                    //    {
                    //        throw new ArgumentException($"Pin {output.Pin} not support mode Output");
                    //    }
                    //}
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.InnerException?.Message ?? ex.Message);
                }
            }
            _logger.LogInformation("*************INIT GPIO DONE*************");
        }

        public void Write(Item item)
        {
            _logger.LogInformation($"WRITE {Newtonsoft.Json.JsonConvert.SerializeObject(item)}");
            if (_settings.Outputs.Any(i => i.Name == item.Name && i.Pin == item.Pin))
            {
                try
                {
                    if (_controller.IsPinOpen(item.Pin))
                    {
                        _controller.Write(item.Pin, item.Value);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.InnerException?.Message ?? ex.Message);
                }
            }
        }

        public async Task WriteAsync(Item item)
        {
            _logger.LogInformation($"WRITE {Newtonsoft.Json.JsonConvert.SerializeObject(item)} with delay {_settings?.Delay}ms");
            if (_settings.Outputs.Any(i => i.Name == item.Name && i.Pin == item.Pin))
            {
                try
                {
                    if (_controller.IsPinOpen(item.Pin))
                    {
                        _controller.Write(item.Pin, PinValue.High);
                        await Task.Delay(_settings.Delay);
                        _controller.Write(item.Pin, PinValue.Low);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.InnerException?.Message ?? ex.Message);
                }
            }
        }

        public List<Item> Read()
        {
            if (_settings == null)
            {
                return null;
            }

            foreach (var item in _settings.Inputs)
            {
                try
                {
                    if (_controller.IsPinOpen(item.Pin))
                    {
                        item.Value = Equals(_controller.Read(item.Pin), PinValue.High);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.InnerException?.Message ?? ex.Message);
                }
            }

            return _settings.Inputs.Union(_settings.Outputs).ToList();
        }

        public Item[] GetOutputs()
        {
            return _settings?.Outputs;
        }
    }
}