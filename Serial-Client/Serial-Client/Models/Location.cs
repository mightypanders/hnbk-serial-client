using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Serial_Client.Models
{
    public class Location
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public virtual List<Position> Positions { get; set; }
    }
}
