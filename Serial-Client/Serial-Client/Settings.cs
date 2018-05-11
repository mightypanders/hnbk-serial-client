using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Serial_Client
{
    public static class Settings
    {
        private static string _dBUrl;
        private static string _standort;
        private static string _raum;
        private static string _rechner;
        private static int _intervall;

        public static string DBUrl { get => _dBUrl; set => _dBUrl = value; }
        public static string Standort { get => _standort; set => _standort = value; }
        public static string Raum { get => _raum; set => _raum = value; }
        public static string Rechner { get => _rechner; set => _rechner = value; }
        public static int Intervall { get => _intervall; set => _intervall = value; }
    }
}
