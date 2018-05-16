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
        private int timeoutcounter = 0;

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
            //Port = InitPort();
        }

        public void doWork()
        {
            this.Running = true;
            this.WorkerState = "Run";
            this.timeoutcounter = 0;
            try
            { Port.Open(); }
            catch (Exception ex)
            {
                Console.WriteLine("Can't open Port");
                Console.WriteLine(ex.Message);
            }

            ReadFromSerial = new Thread(Read);
            IterateOverBuffer = new Thread(IterateOverList);

            try
            {
                ReadFromSerial.Start();
                IterateOverBuffer.Start();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Can't start worker threads");
                Console.WriteLine(ex.Message);
            }
        }
        public void stopWork()
        {
            this.WorkerState = "Stop";
            this.Running = false;
        }
        public string WorkerState
        {
            get
            {
                return _workerState;
            }

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
            get
            {
                return running;
            }

            set
            {
                if (value == running)
                    return;
                running = value;
                OnPropertyChanged("");
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
            try
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

                var position = context.Positions.SingleOrDefault(x => x.PcNumber == Settings.Rechner && x.Room == Settings.Raum);

                if (position == null)
                {
                    position =
                      context.Positions.SingleOrDefault(x => x.Room == Settings.Raum && x.PcNumber == Settings.Rechner) ??
                      new Position()
                      {
                          Room = Settings.Raum,
                          PcNumber = Settings.Rechner
                      };
                }
                data.Position = position;
                data.Position.Location = location;

                context.Measurements.Add(data);
                context.SaveChanges();

            }
            catch (Exception ex)
            {
                Console.WriteLine(String.Format("Error during Write {0}", ex.Message));
                this.stopWork();
            }            //FillGrid();
        }
        private string getStringfromSerialPort()
        {
            string str = "";
            try
            {
                using (Port = InitPort())
                {
                    Port.Open();
                    Port.ReadTimeout = 1000;
                    str = Port.ReadLine().Replace("\r", "");
                    Port.Close();
                }
                if (Port != null)
                {
                    Port.Dispose();
                    Port = null;
                }
            }
            catch (InvalidOperationException inv)
            {
                Console.WriteLine("Can't read from port");
                Console.WriteLine(inv.Message);
                this.stopWork();
            }
            catch (TimeoutException tim)
            {
                Console.WriteLine("Read from Port timed out.");
                Console.WriteLine(tim.Message);
                timeoutcounter++;
                //this.stopWork();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Something else went wrong during read from Port.");
                Console.WriteLine(ex.Message);
                this.stopWork();
            }
            return str;
        }
        private void IterateOverList()
        {
            while (Running)
            {
                if (fifobuffer.Count > 0)
                {
                    //Dispatcher.Invoke(DispatcherPriority.Normal,new DispatcherdDelegate (Write(fifobuffer[0])));
                    try
                    {
                        Write(fifobuffer[0]);
                        fifobuffer.RemoveAt(0);

                    }
                    catch (ArgumentOutOfRangeException oor)
                    {
                        Console.WriteLine("Error during buffer access.");
                        Console.WriteLine(oor.Message);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Unrecognized Error during Buffer read");
                        Console.WriteLine(ex.Message);
                    }
                }
                Thread.Sleep(1);
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
                if (timeoutcounter >= 10)
                    this.stopWork();
            }
            //if (Port.IsOpen)
            //    Port.Close();
            //if (Port != null)
            //{
            //    Port.Dispose();
            //    Port = null;
            //}

        }
    }
}
