using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SagaOrchPattern.DB
{
    public class OrchSagaDbContext : DbContext
    {
        public DbSet<OrderStateData> OrderStateData { get; set; }



        public OrchSagaDbContext()
        {
        }

        public OrchSagaDbContext(DbContextOptions<OrchSagaDbContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=.; initial catalog=OrchSagaDb;integrated security=true;");
        }
    }
}
