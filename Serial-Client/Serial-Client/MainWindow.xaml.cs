using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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
        public DataBase db;
        public SerialPort port;
        public MainWindow()
        {
            db = new DataBase();
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
        }

        private void btnOpenPort_Click(object sender, RoutedEventArgs e)
        {

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
            catch (Exception ex )
            {
                Console.WriteLine(ex.Message);
            }
            this.grDB.DataContext = table.DefaultView;
        }

        private void ListAllComPorts()
        {
            this.cmbPorts.ItemsSource = SerialPort.GetPortNames();
            if (this.cmbPorts.Items.Count >0)
            this.cmbPorts.SelectedIndex = 0;
        }

        private void btnRead_Click(object sender, RoutedEventArgs e)
        {
            string read_Serial_Data = getStringfromSerialPort();
        }

        private string getStringfromSerialPort()
        {
            //throw new NotImplementedException();
            return "";
        }

        private void btnTestWerte_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxButton button = MessageBoxButton.YesNoCancel;
            if (MessageBox.Show("Sicher, dass 100000 Datensätze in die Datenbank geschrieben werden sollen?","Sicher?",button)==MessageBoxResult.OK)
            {
                db.Connection.Open();
                string query = $"delete from {db.Table}";
                SqlCommand cmd = new SqlCommand(query, db.Connection);
                cmd.ExecuteNonQuery();

                TestingData test = new TestingData(100000);
                test.CreateTestData();
                db.Connection.Close();

            }        }
    }
}
