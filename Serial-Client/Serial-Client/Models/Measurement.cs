using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Serial_Client.Models
{
    public class Measurement
    {
        public int ID { get; set; }
        public DateTime Date { get; set; }
        public float Temperature { get; set; }

        public virtual Position Position { get; set; }
        public int PositionId { get; set; }
    }
}
