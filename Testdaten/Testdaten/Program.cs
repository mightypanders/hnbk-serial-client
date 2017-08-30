using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Testdaten
{
    class Program
    {
        static void Main(string[] args)
        {

            // Nicht perfekt, aber zum schnellen Erstellen von Testdaten reicht es...

            TestingData data = new TestingData(10000);
            data.CreateTestData();
        }
    }
}
