using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Serial_Client.Models
{
    class Position
    {
        public int Id { get; set; }
        public string Room { get; set; }
        public string PcNumber { get; set; }

        public int LocationId { get; set; }
        public virtual Location Location { get; set; }
        public virtual List<Measurement> Measurements { get; set; }
    }
}
