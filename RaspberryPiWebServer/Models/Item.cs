using System.Device.Gpio;

namespace RaspberryPiWebServer.Models
{
    public class Item
    {
        public string Name { get; set; }
        public int Pin { get; set; }
        public bool Value { get; set; }
    }
}
