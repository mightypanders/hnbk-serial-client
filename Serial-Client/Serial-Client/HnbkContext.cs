using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Serial_Client.Models;

namespace Serial_Client
{
    class HnbkContext : DbContext
    {
        public HnbkContext() : base("HnbkDBConnectionString")
        {

        }

        public DbSet<Location> Locations { get; set; }
        public DbSet<Position> Positions { get; set; }
        public DbSet<Measurement> Measurements { get; set; }
    }
}
