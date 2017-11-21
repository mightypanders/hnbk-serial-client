using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Serial_Client
{
    class TemperaturDaten
    {
        decimal _temperatur;
        DateTime _datum;
        string _workstation;

        public decimal Temperatur { get => _temperatur; set => _temperatur = value; }
        public DateTime Datum { get => _datum; set => _datum = value; }
        public string Workstation { get => _workstation; set => _workstation = value; }
    }
}
