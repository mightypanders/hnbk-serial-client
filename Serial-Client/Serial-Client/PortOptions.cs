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
        private static string portName="COM1";
        private static int baudRate=9600;
        private static Parity parity = Parity.Even;
        private static int dataBits = 8;
        private static StopBits stopBits = StopBits.One;
        private static Handshake handshake = Handshake.None;

        public static List<int> BaudRateList = new List<int>() { 300, 600, 1200, 9600 };
        public static List<int> DataBitList = new List<int>() { 5,6,7, 8 };

        public static string PortName { get => portName; set => portName = (value!="")?value:"COM1"; }
        public static int BaudRate { get => baudRate; set => baudRate = (value!=0)?value:9600; }
        public static Parity Parity { get => parity; set => parity = value; }
        public static int DataBits { get => dataBits; set => dataBits = (value!=0)?value:8; }
        public static StopBits StopBits { get => stopBits; set => stopBits = (value!=StopBits.None)?value:StopBits.One; }
        public static Handshake Handshake { get => handshake; set => handshake = value; }

    }
}
