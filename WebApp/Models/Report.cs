namespace WebApp.Models
{
    public class Report
    {
        public Report()
        {
            Id = Guid.NewGuid();
        }
        public Guid Id { get; set; }
        public string Template { get; set; }
        public string Model { get; set; }
        public DateTime? StartAt { get; set; }
        public int? Total { get; set; }
        public int? Ok { get; set; }
        public int? Ng { get => Total - Ok; }
        public float? OkRate { get => 1.0f* Ok / ((Total == null || Total == 0) ? 1 : Total); }
        public float? NgRate { get => 1.0f* Ng / ((Total == null || Total == 0) ? 1 : Total); }
    }
}
