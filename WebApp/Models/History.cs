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
        }
        public Guid Id { get; set; }
        public string Model { get; set; }
        public string Code { get; set; }
        public string Template { get; set; }
        public DateTime Created { get; set; }

        [NotMapped]
        public bool isCount { get; set; }
    }
}
