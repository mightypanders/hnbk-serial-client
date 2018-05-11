using Serial_Client.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Serial_Client
{
    public class RWWorker : INotifyPropertyChanged
    {
        private bool running = false;
        private List<string> fifobuffer;
        private SerialPort Port;
        public Thread ReadFromSerial;
        public Thread IterateOverBuffer;
        private string _workerState;

        public event PropertyChangedEventHandler PropertyChanged;
        public virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public RWWorker()
        {
            this.Running = false;
            this.WorkerState = "Stop";
            fifobuffer = new List<string>();
        }
        public void Init()
        {
            Port = InitPort();
        }

        public void doWork()
        {
            this.Running = true;
            this.WorkerState = "Run";
            Port.Open();
            ReadFromSerial = new Thread(Read);
            IterateOverBuffer = new Thread(IterateOverList);
            ReadFromSerial.Start();
            IterateOverBuffer.Start();
        }
        public void stopWork()
        {
            this.WorkerState = "Stop";
            this.Running = false;
        }
        public string WorkerState
        {
            get => _workerState;
            set
            {
                if (value == _workerState)
                    return;
                _workerState = value;
                OnPropertyChanged("WorkerState");
            }
        }
        public bool Running
        {
            get => running;
            set
            {
                if (value == running)
                    return;
                running = value;
                OnPropertyChanged("Running");
            }
        }

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
            Port.ReadTimeout = 100;
            var str = Port.ReadLine().Replace("\r", "");
            return str;
        }
        private void IterateOverList()
        {
            while (Running)
            {
                if (fifobuffer.Count > 0)
                {
                    //Dispatcher.Invoke(DispatcherPriority.Normal,new DispatcherdDelegate (Write(fifobuffer[0])));
                    Write(fifobuffer[0]);
                    fifobuffer.RemoveAt(0);
                }
                Thread.Sleep(100);
            }
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
