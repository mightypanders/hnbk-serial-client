using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using Serial_Client.Models;
using Testdaten;

namespace Serial_Client
{
    /// <summary>
    /// Interaktionslogik für Window1.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool _reading = false;
        private HnbkContext ctx = new HnbkContext();
        private int _ThreadInterval = 1;

        public SerialPort Port;
        public Thread ReadFromSerial;
        public Thread IterateOverBuffer;
        public List<string> fifobuffer = new List<string>();

        public int ThreadInterval { get => _ThreadInterval; set => _ThreadInterval = value; }
        public string txtLocationString = "";
        public string txtRoomString = "";
        public string txtNameString = "";

        public MainWindow()
        {
            var bla = PortOptions.DataBits;
            Port = InitPort();
            InitializeComponent();
            ListAllComPorts();
            FillBoxes();
#if (DEBUG)
            this.btnTestWerte.Visibility = Visibility.Visible;
#endif
        }

        private void FillBoxes()
        {
            this.cmbHandshake.ItemsSource = Enum.GetValues(typeof(Handshake)).Cast<Handshake>();
            this.cmbHandshake.SelectedValue = Handshake.None;
            this.cmbBaud.ItemsSource = PortOptions.BaudRateList;
            this.cmbBaud.SelectedValue = 9600;
            this.cmbStop.ItemsSource = Enum.GetValues(typeof(StopBits)).Cast<StopBits>();
            this.cmbStop.SelectedValue = StopBits.One;
            this.cmbDatabit.ItemsSource = PortOptions.DataBitList;
            this.cmbDatabit.SelectedValue = 8;
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            btnStart.IsEnabled = false;
            btnStop.IsEnabled = true;
            _reading = true;

            portOptionsZuweisen();

            ReadFromSerial = new Thread(this.Read);
            IterateOverBuffer = new Thread(this.IterateOverList);

            ReadFromSerial.Start();
            IterateOverBuffer.Start();
        }

        private void portOptionsZuweisen()
        {
            if (!this.Port.IsOpen)
            {
                PortOptions.PortName = this.cmbPorts.SelectedValue.ToString();
                PortOptions.BaudRate = int.Parse(this.cmbBaud.SelectedValue.ToString());
                PortOptions.Handshake = (Handshake)this.cmbHandshake.SelectedItem;
                PortOptions.StopBits = (StopBits)this.cmbStop.SelectedItem;
                PortOptions.DataBits = int.Parse(this.cmbDatabit.SelectedValue.ToString());
                Port = InitPort();
                Port.Open();
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

        private void btnFillgrid_Click(object sender, RoutedEventArgs e)
        {
            FillGrid();
        }

        private void FillGrid()
        {
            this.grDB.DataContext = null;
            grDB.DataContext = ctx.Measurements.Select(x => new
            {
                Standort = x.Position.Location.Name,
                Raum = x.Position.Room,
                PC = x.Position.PcNumber,
                Temperatur = x.Temperature,
                Datum = x.Date
            }).ToList();
        }

        private void ListAllComPorts()
        {
            this.cmbPorts.ItemsSource = SerialPort.GetPortNames();
            if (this.cmbPorts.Items.Count > 0)
                this.cmbPorts.SelectedIndex = 0;
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            btnStart.IsEnabled = true;
            btnStop.IsEnabled = false;
            _reading = false;

            if (this.Port.IsOpen)
            {
                Port.Close();
            }
        }
        public void txtLocation_TextChanged(object sender, EventArgs e)
        {
            txtLocationString = txtStandort.Text;
        }
        public void txtRoom_TextChanged(object sender, EventArgs e)
        {
            txtRoomString = txtRaum.Text;
        }
        public void txtName_TextChanged(object sender, EventArgs e)
        {
            txtNameString = txtPCName.Text;
        }
        private string getStringfromSerialPort()
        {
            var str = Port.ReadLine().Replace("\r", "");
            return str;
        }

        private void Write(string message)
        {
            Measurement data = new Measurement()
            {
                Temperature = float.Parse(message.Replace(".", ",")),
                Date = DateTime.Now
            };
            HnbkContext context = new HnbkContext();
            var location = context.Locations.FirstOrDefault(x => x.Name == txtLocationString) ?? new Location()
            {
                Name = txtLocationString
            };

            var position =
                context.Positions.SingleOrDefault(x => x.Room == txtRoomString && x.PcNumber == txtNameString) ??
                new Position()
                {
                    Room = txtRoomString,
                    PcNumber = txtNameString
                };

            data.Position = position;
            data.Position.Location = location;

            context.Measurements.Add(data);
            context.SaveChanges();
            //FillGrid();
        }

        private void IterateOverList()
        {
            while (true)
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
            while (_reading)
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
                Thread.Sleep(ThreadInterval * 1000);
            }
        }

        private void btnTestWerte_Click(object sender, RoutedEventArgs e)
        {
            //MessageBoxButton button = MessageBoxButton.YesNoCancel;
            //if (MessageBox.Show("Sicher, dass 100000 Datensätze in die Datenbank geschrieben werden sollen?", "Sicher?", button) == MessageBoxResult.OK)
            //{
            //    db.Connection.Open();
            //    try
            //    {
            //        string query = $"delete from {db.Table}";
            //        SqlCommand cmd = new SqlCommand(query, db.Connection);
            //        cmd.ExecuteNonQuery();

            //        TestingData test = new TestingData(100000);
            //        test.CreateTestData();

            //    }
            //    catch (Exception ex)
            //    {
            //        MessageBox.Show(ex.Message);
            //        throw;
            //    }
            //    finally { db.Connection.Close(); }

            //}
        }

        private void txtInterval_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (int.TryParse(txtInterval.Text, out int value))
                {
                    ThreadInterval = value;
                }
                else
                {
                    txtInterval.Text = "";
                }
            }
        }
    }
}
