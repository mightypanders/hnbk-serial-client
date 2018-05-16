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
        private static string _dBName;
        private static string _dbUser;
        private static string _dbPass;
        private static string _standort;
        private static string _raum;
        private static string _rechner;
        private static int _intervall;

        public static string DBUrl
        {
            get { return _dBUrl; }
            set
            {
                _dBUrl = value;
            }
        }
        public static string Standort
        {
            get { return _standort; }
            set
            {
                _standort = value;
            }
        }
        public static string Raum
        {
            get { return _raum; }
            set
            {
                _raum = value;
            }
        }
        public static string Rechner
        {
            get { return _rechner; }
            set
            {
                _rechner = value;
            }
        }
        public static int Intervall
        {
            get
            {
                return _intervall;
            }

            set
            {
                _intervall = value;
            }
        }
        public static string DBName
        {
            get
            {
                return _dBName;
            }

            set
            {
                _dBName = value;
            }
        }
        public static string DbUser
        {
            get
            {
                return _dbUser;
            }

            set
            {
                _dbUser = value;
            }
        }
        public static string DbPass
        {
            get
            {
                return _dbPass;
            }

            set
            {
                _dbPass = value;
            }
        }
    }
}
