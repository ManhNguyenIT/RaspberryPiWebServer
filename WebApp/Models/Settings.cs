namespace WebApp.Models
{
    public class Settings
    {
        public Item[] Inputs { get; set; }
        public Item[] Outputs { get; set; }
        public int Delay { get; set; }
        public string Password { get; set; }
    }
}
