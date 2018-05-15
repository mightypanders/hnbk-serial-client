using Serial_Client;
using Serial_Client.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics.SymbolStore;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Serial_Client
{
    class TestingData
    {
        private int counter;

        DataBase db = new DataBase();
        Temperature temperature = new Temperature();
        Brightness brightness = new Brightness();
        Humidity humidity = new Humidity();
        WindVelocity windVelocity = new WindVelocity();
        Date date = new Date(true);

        public TestingData(int counter = 100)
        {
            this.Counter = counter;
        }

        public int Counter { get => counter; set => counter = value; }
        public DataBase Db { get => db; set => db = value; }
        public Temperature Temperature { get => temperature; set => temperature = value; }
        public Brightness Brightness { get => brightness; set => brightness = value; }
        public Humidity Humidity { get => humidity; set => humidity = value; }
        public WindVelocity WindVelocity { get => windVelocity; set => windVelocity = value; }
        public Date Date { get => date; set => date = value; }

        public void CreateTestData()
        {
            Db.Connection.ConnectionString = $"Data Source={Db.ServerName};Initial Catalog={Db.Database};User id={Db.User};Password={Db.Password};";

            if (Date.LastDateOfDatabase)
            {
                SqlCommand cmd = new SqlCommand($"SELECT MAX(Datum) FROM {Db.Table}", Db.Connection);
                try
                {
                    Db.Connection.Open();
                    Date.StartDate = (DateTime)cmd.ExecuteScalar();
                    Date.StartDate = Date.StartDate.AddMinutes(Date.IncrementMinutes);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                Db.Connection.Close();
            }

            String query = $"INSERT INTO {Db.Table} (Temperatur,Luftfeuchtigkeit,Windgeschwindigkeit,Helligkeit,Datum) VALUES (@Temperatur,@Luftfeuchtigkeit,@Windgeschwindigkeit,@Helligkeit,@Datum)";

            SqlCommand command = new SqlCommand(query, Db.Connection);
            Random rnd = new Random();

            db.Connection.Open();

            for (int i = 0; i < Counter; i++)
            {
                Temperature.BaseTemperature += rnd.Next(Temperature.Decrement, Temperature.Increment + 1);
                Temperature.BaseTemperature = (Temperature.BaseTemperature < -50) ? -50 : Temperature.BaseTemperature;
                Temperature.BaseTemperature = (Temperature.BaseTemperature > 50) ? 50 : Temperature.BaseTemperature;

                Brightness.BaseBrightness += rnd.Next(Brightness.Decrement, Brightness.Increment + 1);
                Brightness.BaseBrightness = (Brightness.BaseBrightness < 0) ? 0 : Brightness.BaseBrightness;
                Brightness.BaseBrightness = (Brightness.BaseBrightness > 25000) ? 25000 : Brightness.BaseBrightness;

                Humidity.BaseHumidity += rnd.Next(Humidity.Decrement, Humidity.Increment + 1);
                Humidity.BaseHumidity = (Humidity.BaseHumidity < 0) ? 0 : Humidity.BaseHumidity;
                Humidity.BaseHumidity = (Humidity.BaseHumidity > 100) ? 100 : Humidity.BaseHumidity;

                WindVelocity.BaseWindVelocity += rnd.Next(WindVelocity.Decrement, WindVelocity.Increment + 1);
                WindVelocity.BaseWindVelocity = (WindVelocity.BaseWindVelocity < 0) ? 0 : WindVelocity.BaseWindVelocity;
                WindVelocity.BaseWindVelocity = (WindVelocity.BaseWindVelocity < 200) ? 200 : WindVelocity.BaseWindVelocity;

                command.Parameters.AddWithValue("@Temperatur", Temperature.BaseTemperature);
                command.Parameters.AddWithValue("@Luftfeuchtigkeit", Humidity.BaseHumidity);
                command.Parameters.AddWithValue("@Windgeschwindigkeit", WindVelocity.BaseWindVelocity);
                command.Parameters.AddWithValue("@Helligkeit", Brightness.BaseBrightness);
                command.Parameters.AddWithValue("@Datum", Date.StartDate);

                Console.WriteLine("Schreibe Datensatz " + i);
                command.ExecuteNonQuery();
                command.Parameters.Clear();
                Date.StartDate = Date.StartDate.AddMinutes(Date.IncrementMinutes);
            }
            db.Connection.Close();

        }

    }

    public class DataBase
    {
        private SqlConnection connection = new SqlConnection();
        private string serverName = "sven.tech";
        private string database = "hnbkwetter";
        private string user = "hnbkserialclient";
        private string password = "hnbkserial";
        private string table = "Wetter";

        public string ServerName { get => serverName; set => serverName = value; }
        public string Database { get => database; set => database = value; }
        public string User { get => user; set => user = value; }
        public string Password { get => password; set => password = value; }
        public string Table { get => table; set => table = value; }
        public SqlConnection Connection { get => connection; set => connection = value; }
        public DataBase()
        {
            Connection.ConnectionString = $"Data Source={this.ServerName};Initial Catalog={this.Database};User id={this.User};Password={this.Password};";
        }
    }

    public class Temperature
    {
        private int baseTemperature;
        private int increment;
        private int decrement;

        public Temperature(int BaseTemperature = 20, int Increment = 2, int Decrement = -2)
        {
            this.BaseTemperature = BaseTemperature;
            this.Increment = Increment;
            this.Decrement = Decrement;
        }

        public int BaseTemperature { get => baseTemperature; set => baseTemperature = value; }
        public int Increment { get => increment; set => increment = value; }
        public int Decrement { get => decrement; set => decrement = value; }
    }

    public class Brightness
    {
        private int baseBrightness;
        private int increment;
        private int decrement;

        public Brightness(int BaseBrightness = 20000, int Increment = 100, int Decrement = -100)
        {
            this.BaseBrightness = BaseBrightness;
            this.Increment = Increment;
            this.Decrement = Decrement;
        }

        public int BaseBrightness { get => baseBrightness; set => baseBrightness = value; }
        public int Increment { get => increment; set => increment = value; }
        public int Decrement { get => decrement; set => decrement = value; }
    }

    public class Humidity
    {
        private int baseHumidity;
        private int increment;
        private int decrement;

        public Humidity(int BaseHumidity = 20, int Increment = 3, int Decrement = -3)
        {
            this.BaseHumidity = BaseHumidity;
            this.Increment = Increment;
            this.Decrement = Decrement;
        }

        public int BaseHumidity { get => baseHumidity; set => baseHumidity = value; }
        public int Increment { get => increment; set => increment = value; }
        public int Decrement { get => decrement; set => decrement = value; }
    }

    public class WindVelocity
    {
        private int baseWindVelocity;
        private int increment;
        private int decrement;

        public WindVelocity(int BaseWindVelocity = 0, int Increment = 5, int Decrement = -5)
        {
            this.BaseWindVelocity = BaseWindVelocity;
            this.Increment = Increment;
            this.Decrement = Decrement;
        }

        public int BaseWindVelocity { get => baseWindVelocity; set => baseWindVelocity = value; }
        public int Increment { get => increment; set => increment = value; }
        public int Decrement { get => decrement; set => decrement = value; }
    }

    public class Date
    {
        private DateTime startDate = DateTime.Now;
        private int incrementMinutes = 5;
        private bool lastDateOfDatabase = true;

        public Date(bool lastDateOfDatabase)
        {
            this.LastDateOfDatabase = lastDateOfDatabase;
        }

        public DateTime StartDate { get => startDate; set => startDate = value; }
        public int IncrementMinutes { get => incrementMinutes; set => incrementMinutes = value; }
        public bool LastDateOfDatabase { get => lastDateOfDatabase; set => lastDateOfDatabase = value; }
    }

    public static class TestGenerator
    {

        public static void Generate(HnbkContext ctx, DateTime startDate, DateTime endDate, TestInvervall intervall)
        {
            Random r = new Random();

            var locations = ctx.Locations.ToList();

            while (startDate <= endDate)
            {
                switch (intervall)
                {
                    case TestInvervall.Milliseconds:
                        startDate = startDate.AddMilliseconds(1);
                        break;
                    case TestInvervall.Seconds:
                        startDate = startDate.AddSeconds(1);
                        break;
                    case TestInvervall.Minutes:
                        startDate = startDate.AddMinutes(1);
                        break;
                    case TestInvervall.Hours:
                        startDate = startDate.AddHours(1);
                        break;
                    case TestInvervall.Days:
                        startDate = startDate.AddDays(1);
                        break;
                    case TestInvervall.Months:
                        startDate = startDate.AddMonths(1);
                        break;
                    case TestInvervall.Years:
                        startDate = startDate.AddYears(1);
                        break;
                    default:
                        break;
                }

                if (locations.Count == 0)
                {
                    locations.Add(new Location()
                    {
                        Name = "Testraum"
                    });
                }

                for (int i = 0; i < locations.Count; i++)
                {
                    if (locations[i].Positions.Count == 0)
                    {
                        locations[i].Positions.Add(new Position()
                        {
                            Location = locations[i],
                            PcNumber = "PC" + (i + 1),
                            Room = "HNN10"+ (i + 1)
                        });
                    }

                    Measurement data = new Measurement
                    {
                        Temperature = (NextFloat(r)),
                        Date = startDate,
                        Position = locations[i].Positions[0]
                    };
                    data.Position.Location = locations[i];
                    ctx.Measurements.Add(data);
                }
            }
                ctx.SaveChanges();
        }

        static float NextFloat(Random random)
        {
            double mantissa = (random.NextDouble() * 2.0);
            double exponent = random.Next(15, 30);
            return (float)(mantissa + exponent);
        }
    }
}
