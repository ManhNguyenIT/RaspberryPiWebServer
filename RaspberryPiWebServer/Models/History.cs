using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RaspberryPiWebServer.Models
{
    public class History
    {
        public History()
        {
            Id = Guid.NewGuid();
            CreateDate = DateTime.Now.Date;
        }
        public Guid Id { get; set; }
        public Guid ModelId { get; set; }
        public string Code { get; set; }
        public string Template { get; set; }
        public bool IsOk { get; set; }
        public DateTime CreateDate { get; set; }

        public virtual Model Model { get; set; }
    }
}
