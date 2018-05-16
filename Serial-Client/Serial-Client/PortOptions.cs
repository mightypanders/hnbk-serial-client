using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Serial_Client
{
    public static class PortOptions
    {
        private static string portName = "COM1";
        private static int baudRate = 9600;
        private static Parity parity = Parity.Even;
        private static int dataBits = 8;
        private static StopBits stopBits = StopBits.One;
        private static Handshake handshake = Handshake.None;
        private static string[] portNames = new string[1];

        private static List<int> baudRateList = new List<int>() { 300, 600, 1200, 9600 };
        private static List<int> dataBitList = new List<int>() { 5, 6, 7, 8 };

        public static string PortName
        {
            get
            {
                return portName;
            }

            set
            {
                portName = (value != "") ? value : "COM1";
            }
        }
        public static int BaudRate
        {
            get
            {
                return baudRate;
            }

            set
            {
                baudRate = (value != 0) ? value : 9600;
            }
        }
        public static Parity Parity
        {
            get
            {
                return parity;
            }

            set
            {
                parity = value;
            }
        }
        public static int DataBits
        {
            get
            {
                return dataBits;
            }

            set
            {
                dataBits = (value != 0) ? value : 8;
            }
        }
        public static StopBits StopBits
        {
            get
            {
                return stopBits;
            }

            set
            {
                stopBits = (value != StopBits.None) ? value : StopBits.One;
            }
        }
        public static Handshake Handshake
        {
            get
            {
                return handshake;
            }

            set
            {
                handshake = value;
            }
        }
        public static List<int> BaudRateList
        {
            get
            {
                return baudRateList;
            }

            set
            {
                baudRateList = value;
            }
        }
        public static List<int> DataBitList
        {
            get
            {
                return dataBitList;
            }

            set
            {
                dataBitList = value;
            }
        }

        public static string[] PortNames
        {
            get
            {
                return portNames;
            }

            set
            {
                portNames = value;
            }
        }

        public static void ListAllComPorts()
        {
            PortNames = SerialPort.GetPortNames();
        }
    }
}
