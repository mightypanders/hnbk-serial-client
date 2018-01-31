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
        //private List<TemperaturDaten> _buffer;
        public SerialPort Port;
        public MainWindow()
        {
            var bla = PortOptions.DataBits;
            Port = InitPort();
            InitializeComponent();
            ListAllComPorts();
            FillBoxes();
        }

        private void FillBoxes()
        {
            this.cmbParity.ItemsSource = Enum.GetValues(typeof(Parity)).Cast<Parity>();
            this.cmbParity.SelectedValue = Parity.Even;
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

            if (!this.Port.IsOpen)
            {
                PortOptions.PortName = this.cmbPorts.SelectedValue.ToString();
                PortOptions.BaudRate = int.Parse(this.cmbBaud.SelectedValue.ToString());
                PortOptions.Handshake = (Handshake)this.cmbHandshake.SelectedItem;
                PortOptions.Parity = (Parity)this.cmbParity.SelectedItem;
                PortOptions.StopBits = (StopBits)this.cmbStop.SelectedItem;
                PortOptions.DataBits = int.Parse(this.cmbDatabit.SelectedValue.ToString());
                Port = InitPort();
                Port.Open();
            }

            Read();
        }

        private SerialPort InitPort()
        {
            SerialPort port = new SerialPort();
            port.PortName = PortOptions.PortName;
            port.DataBits = PortOptions.DataBits;
            port.Handshake = PortOptions.Handshake;
            port.BaudRate = PortOptions.BaudRate;
            port.Parity = PortOptions.Parity;
            port.StopBits = PortOptions.StopBits;
            port.DataReceived += new SerialDataReceivedEventHandler(this.DataReceived);
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
            //string read_Serial_Data = getStringfromSerialPort();
        }

        private string getStringfromSerialPort()
        {
            //throw new NotImplementedException();
            string retString = Port.ReadLine();
            return "";
        }

        private void DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
        }

        private void Write(string message)
        {
            Measurement data = new Measurement()
            {
                Temperature = float.Parse(message),
                Date = DateTime.Now
            };

            var location = ctx.Locations.SingleOrDefault(x => x.Name == txtStandort.Text) ?? new Location()
            {
                Name = txtStandort.Text
            };

            var position =
                ctx.Positions.SingleOrDefault(x => x.Room == txtRaum.Text && x.PcNumber == txtPCName.Text) ??
                new Position()
                {
                    Room = txtRaum.Text,
                    PcNumber = txtPCName.Text
                };

            data.Position = position;
            data.Position.Location = location;

            ctx.Measurements.Add(data);
            ctx.SaveChanges();
        }



        private void Read()
        {
            while (_reading)
            {
                try
                {
                    string message = Port.ReadLine();

                    if (!string.IsNullOrEmpty(message))
                    {
                        Write(message);

                        Console.WriteLine(message);
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                }
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
    }
}
