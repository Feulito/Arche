using IOC.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class AbstractEntity : IEntity
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public DateTime Creation { get; set; } = DateTime.Now;
        public DateTime Update { get; set; }
        public bool Deleted { get; set; } = false;
    }
}
