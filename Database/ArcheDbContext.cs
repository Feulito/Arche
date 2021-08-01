using Core;
using Core.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database
{
    public class ArcheDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public ArcheDbContext(DbContextOptions<ArcheDbContext> options) : base(options) { }

    }
}
