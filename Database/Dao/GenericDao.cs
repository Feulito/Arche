using IOC.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.Dao
{
    public class GenericDao<T> : AbstractDao<T> where T : class, IEntity, new()
    {
        public GenericDao(ArcheDbContext dbContext) : base(dbContext) { }
    }
}
