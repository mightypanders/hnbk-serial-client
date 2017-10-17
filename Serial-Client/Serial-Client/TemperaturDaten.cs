using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Serial_Client
{
    class TemperaturDaten
    {
        string _temperatur;
        string _luftfeuchtigkeit;
        string _windgeschwindigkeit;
        string _helligkeit;
        DateTime _datum;

        public string Temperatur { get => _temperatur; set => _temperatur = value; }
        public string Luftfeuchtigkeit { get => _luftfeuchtigkeit; set => _luftfeuchtigkeit = value; }
        public string Windgeschwindigkeit { get => _windgeschwindigkeit; set => _windgeschwindigkeit = value; }
        public string Helligkeit { get => _helligkeit; set => _helligkeit = value; }
        public DateTime Datum { get => _datum; set => _datum = value; }
    }
}
