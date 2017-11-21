using System;
using System.Collections.Generic;
using System.Data;
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
using Testdaten;

namespace Serial_Client
{
    /// <summary>
    /// Interaktionslogik für Window1.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool _continue = false;
        private List<TemperaturDaten> _buffer;
        public DataBase db;
        public SerialPort port;
        public MainWindow()
        {
            var bla = PortOptions.DataBits;
            db = new DataBase();
            port = initPort();
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

        private void btnOpenPort_Click(object sender, RoutedEventArgs e)
        {

            if (!this.port.IsOpen)
            {
                PortOptions.PortName = this.cmbPorts.SelectedValue.ToString();
                PortOptions.BaudRate = int.Parse(this.cmbBaud.SelectedValue.ToString());
                PortOptions.Handshake = (Handshake)this.cmbHandshake.SelectedItem;
                PortOptions.Parity = (Parity)this.cmbParity.SelectedItem;
                PortOptions.StopBits = (StopBits)this.cmbStop.SelectedItem;
                PortOptions.DataBits = int.Parse(this.cmbDatabit.SelectedValue.ToString());
                port = initPort();
                port.Open();
            }
        }

        private SerialPort initPort()
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
            SqlCommand cmd = new SqlCommand($"Select * from {db.Table}", db.Connection);
            DataTable table = new DataTable();
            try
            {
                db.Connection.Open();
                var tableContent = cmd.ExecuteReader();
                table.Load(tableContent);
                tableContent.Close();
                db.Connection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            this.grDB.DataContext = table.DefaultView;
        }

        private void ListAllComPorts()
        {
            this.cmbPorts.ItemsSource = SerialPort.GetPortNames();
            if (this.cmbPorts.Items.Count > 0)
                this.cmbPorts.SelectedIndex = 0;
        }

        private void btnRead_Click(object sender, RoutedEventArgs e)
        {
            string read_Serial_Data = getStringfromSerialPort();
        }

        private string getStringfromSerialPort()
        {
            //throw new NotImplementedException();
            string retString = port.ReadLine();
            return "";
        }

        private void DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
        }
        public void Write()
        {
            string Dateiname;
            Dateiname = DateTime.Now.ToString().Replace('.', '_');
            Dateiname = Dateiname.Replace(':', '_') + ".txt";
            
            Dateiname = AppDomain.CurrentDomain.BaseDirectory + Dateiname;
            StreamWriter tmpLog = File.AppendText(Dateiname);
            while (_continue)
            {
                if (_buffer.Count > 0)
                {
                    try
                    {
                        tmpLog.WriteLine(_buffer.First().ToString());
                        _buffer.RemoveAt(0);

                        //Thread.Sleep(1000);
                        tmpLog.Flush();
                    }
                    catch (TimeoutException)
                    {
                        Debug.WriteLine("Could not write to file");
                    }
                }
            }
            tmpLog.Close();
        }
        public void Read()
        {
            while (_continue)
            {
                try
                {
                    string message = port.ReadLine();
                    TemperaturDaten data = new TemperaturDaten()
                    {
                        Temperatur = decimal.Parse(message.Remove(message.Length - 1).Replace(".", ",")),
                        Datum = DateTime.Now
                    };
                    Console.WriteLine(message);
                    _buffer.Add(data);

                    //Thread.Sleep(1000);
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                }
            }
        }


        private void btnTestWerte_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxButton button = MessageBoxButton.YesNoCancel;
            if (MessageBox.Show("Sicher, dass 100000 Datensätze in die Datenbank geschrieben werden sollen?", "Sicher?", button) == MessageBoxResult.OK)
            {
                db.Connection.Open();
                try
                {
                    string query = $"delete from {db.Table}";
                    SqlCommand cmd = new SqlCommand(query, db.Connection);
                    cmd.ExecuteNonQuery();

                    TestingData test = new TestingData(100000);
                    test.CreateTestData();

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    throw;
                }
                finally { db.Connection.Close(); }

            }
        }
    }
}
