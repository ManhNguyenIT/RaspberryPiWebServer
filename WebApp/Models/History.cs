using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WebApp.Models
{
    [JsonObject(IsReference = true)]
    public class History
    {
        public History()
        {
            Id = Guid.NewGuid();
            Created = DateTime.Now;
            IsCancelled = false;
        }
        public Guid Id { get; set; }
        public string Model { get; set; }
        public int Times { get; set; }
        public string Code { get; set; }
        public string Template { get; set; }
        public bool IsCancelled { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }

        [NotMapped]
        public string Name { get; set; }

        [NotMapped]
        public int Pin { get; set; }
    }
}
