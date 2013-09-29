using System;
using System.Collections.Generic;

using System.Text;

namespace CdgNetPersistenciaV3
{
    public class Tester
    {

        public Tester()
        {
            TestSQLServer.TestOTDperformance();
        }

    }


    public class TestSQLServer
    {

        public static void TestSelect()
        {
            //SQLServerUtiles oUtileria = new SQLServerUtiles("10.75.1.1", "mcc", 600);

        }


        public static void TestOTDperformance()
        {
            System.Console.WriteLine("========================================================");
            System.Console.WriteLine("  Inicia el Test de Performace de OTDs");
            System.Console.WriteLine("========================================================");
            System.Console.WriteLine("");
            System.Console.WriteLine("");

            List<OTDTester> lista = new List<OTDTester>();

            System.Diagnostics.Stopwatch stopWatch = new System.Diagnostics.Stopwatch();
            stopWatch.Start();
            
            for (int i = 0; i < 1000; i++) lista.Add(new OTDTester(i, ""));

            stopWatch.Stop();

            // Get the elapsed time as a TimeSpan value.
            TimeSpan ts = stopWatch.Elapsed;



            System.Console.WriteLine("========================================================");
            System.Console.WriteLine(string.Format(" Tiempo transcurrido: {0}", ts));
            System.Console.WriteLine("========================================================");
            System.Console.ReadLine();
        }
    }


    public class OTDTester : CdgNetPersistenciaV3.ClasesBases.OTDbase
    {
        public OTDTester(long nId, string cDescripcion)
            :base(nId, cDescripcion)
        {
            //_Set_campos<OTDTester>();
        }
    }

}
