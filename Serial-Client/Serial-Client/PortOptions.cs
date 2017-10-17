using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Serial_Client
{
    static class PortOptions
    {
        private static string portName;
        private static int baudRate;
        private static Parity parity;
        private static int dataBits;
        private static StopBits stopBits;
        private static Handshake handshake;

        public static List<int> BaudRateList = new List<int>() { 300, 600, 1200, 9600 };
        public static string PortName { get => portName; set => portName = (value!="")?value:"COM1"; }
        public static int BaudRate { get => baudRate; set => baudRate = (value!=0)?value:9600; }
        public static Parity Parity { get => parity; set => parity = value; }
        public static int DataBits { get => dataBits; set => dataBits = (value!=0)?value:8; }
        public static StopBits StopBits { get => stopBits; set => stopBits = value; }
        public static Handshake Handshake { get => handshake; set => handshake = value; }

        public static void assignOptionsToPort(SerialPort port)
        {
            port.PortName = PortName;
            port.BaudRate = BaudRate;
            port.Parity = Parity;
            port.DataBits = DataBits;
            port.StopBits = StopBits;
            port.Handshake = Handshake;
        }
    }
}
