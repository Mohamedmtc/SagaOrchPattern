
using Microsoft.EntityFrameworkCore;

using SagaOrchPattern.Order.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SagaOrchPattern.Order.Infra
{
    public class OrderDbContext : DbContext
    {
        public DbSet<OrderPrice> OrderPrices { get; set; }
        public DbSet<OutBox> OutBoxs { get; set; }

        public OrderDbContext()
        {
        }

        public OrderDbContext(DbContextOptions<OrderDbContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=.;initial catalog=OrderpriceDb;integrated security=true;");
        }
    }
}
