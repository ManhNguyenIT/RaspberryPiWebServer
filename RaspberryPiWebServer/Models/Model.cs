using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace RaspberryPiWebServer.Models
{
    public class Model
    {
        public Model()
        {
            Id = Guid.NewGuid();
            Histories = new HashSet<History>();
        }
        public Guid Id { get; set; }
        public string Name { get; set; }

        [JsonIgnore]
        public virtual ICollection<History> Histories { get; set; }
    }
}
