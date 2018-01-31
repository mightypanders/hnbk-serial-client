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
        string _rechnername;
        string _pcnumber;
        string _standort;
        

        public decimal Temperatur { get => _temperatur; set => _temperatur = value; }
        public DateTime Datum { get => _datum; set => _datum = value; }
        public string Workstation { get => _workstation; set => _workstation = value; }
        public string Rechnername { get => _rechnername; set => _rechnername = value; }
        public string Pcnumber { get => _pcnumber; set => _pcnumber = value; }
        public string Standort { get => _standort; set => _standort = value; }
    }
}
