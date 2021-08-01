using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IOC.Data.Interfaces
{
    public interface IEntity
    {
        string Id { get; set; }
        public DateTime Creation { get; set; }
        public DateTime Update { get; set; }
        public bool Deleted { get; set; }
    }
}
