using Serial_Client.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Serial_Client
{
    public class RWWorker
    {
        private bool running = false;
        private List<string> fifobuffer;
        private SerialPort Port;

        public RWWorker()
        {
            Port = InitPort();
        }

        public void doWork()
        {

        }
        public bool Running { get => running; set => running = value; }

        private SerialPort InitPort()
        {
            SerialPort port = new SerialPort
            {
                PortName = PortOptions.PortName,
                DataBits = PortOptions.DataBits,
                Handshake = PortOptions.Handshake,
                BaudRate = PortOptions.BaudRate,
                StopBits = PortOptions.StopBits
            };
            return port;
        }

        private void Write(string message)
        {
            Measurement data = new Measurement()
            {
                Temperature = float.Parse(message.Replace(".", ",")),
                Date = DateTime.Now
            };
            HnbkContext context = new HnbkContext();
            var location = context.Locations.FirstOrDefault(x => x.Name == Settings.Standort) ?? new Location()
            {
                Name = Settings.Standort
            };

            var position =
                context.Positions.SingleOrDefault(x => x.Room == Settings.Raum && x.PcNumber == Settings.Rechner) ??
                new Position()
                {
                    Room = Settings.Raum,
                    PcNumber = Settings.Rechner
                };

            data.Position = position;
            data.Position.Location = location;

            context.Measurements.Add(data);
            context.SaveChanges();
            //FillGrid();
        }
        private string getStringfromSerialPort()
        {
            var str = Port.ReadLine().Replace("\r", "");
            return str;
        }
        private void Read()
        {
            string message = "";
            while (Running)
            {
                try
                {
                    message = getStringfromSerialPort();
                    if (!string.IsNullOrEmpty(message))
                    {
                        fifobuffer.Add(message);
                        //Write(message);
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                }
                Thread.Sleep(Settings.Intervall * 1000);
            }
        }
    }
}
