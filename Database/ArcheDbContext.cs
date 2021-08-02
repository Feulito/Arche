using Core;
using Core.Entities;
using Core.Entities.Site;
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
        public DbSet<Article> Articles { get; set; }
        public ArcheDbContext(DbContextOptions<ArcheDbContext> options) : base(options) { }

    }
}
