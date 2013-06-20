using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Testing
{
    class Program
    {
        static void Main(string[] args)
        {

            //TestSQLServer.TestOTDperformance(); //promedio .0097613

            TestSQLServer.TestADMperformance(); //promedio .694966
        }
    }



    public class TestSQLServer
    {

        public static void TestOTDperformance()
        {
            System.Console.WriteLine("========================================================");
            System.Console.WriteLine("  Inicia el Test de Performace de OTDs");
            System.Console.WriteLine("========================================================");
            System.Console.WriteLine("");
            System.Console.WriteLine("");

            List<TipoClienteOTD> lista = new List<TipoClienteOTD>();

            System.Diagnostics.Stopwatch stopWatch = new System.Diagnostics.Stopwatch();
            stopWatch.Start();

            for (int i = 0; i < 1000; i++) lista.Add(new TipoClienteOTD(i, ""));

            stopWatch.Stop();

            // Get the elapsed time as a TimeSpan value.
            TimeSpan ts = stopWatch.Elapsed;



            System.Console.WriteLine("========================================================");
            System.Console.WriteLine(string.Format(" Tiempo transcurrido: {0}", ts));
            System.Console.WriteLine("========================================================");
            System.Console.ReadLine();

        }

        public static void TestADMperformance()
        {
            System.Console.WriteLine("========================================================");
            System.Console.WriteLine("  Inicia el Test de Performace de ADMs");
            System.Console.WriteLine("========================================================");
            System.Console.WriteLine("");
            System.Console.WriteLine("");

            List<TiposClientesADM> lista = new List<TiposClientesADM>();
            List<TipoClienteOTD> listaOTD = new List<TipoClienteOTD>();

            System.Diagnostics.Stopwatch stopWatch = new System.Diagnostics.Stopwatch();
            stopWatch.Start();

            var oConexion = new CdgNetPersistenciaV3.SQLServerUtiles("10.75.1.1", "mcc", 120);
            oConexion.lConectar();

            for (int i = 0; i < 1000; i++) lista.Add(new TiposClientesADM(oConexion));

            foreach (TiposClientesADM oADM in lista)
            {
                listaOTD.AddRange(oADM.lGet_elementos(string.Empty, new Dictionary<string, object>())[1] as List<TipoClienteOTD>);
            }

            stopWatch.Stop();

            // Get the elapsed time as a TimeSpan value.
            TimeSpan ts = stopWatch.Elapsed;

            System.Console.WriteLine("========================================================");
            System.Console.WriteLine(string.Format(" Instancias recuperadas: {0}", listaOTD.Count()));
            System.Console.WriteLine(string.Format(" Instancia ejemplo: {0}", listaOTD[822]));
            System.Console.WriteLine(string.Format(" Tiempo transcurrido: {0}", ts));
            System.Console.WriteLine("========================================================");
            System.Console.ReadLine();

        }

    }

    [CdgNetPersistenciaV3.Atributos.Tabla("STKTIPOSCLIENTE", CdgNetPersistenciaV3.Atributos.Tabla.SGBD.SQL_SERVER)]
    public class TipoClienteOTD : CdgNetPersistenciaV3.ClasesBases.OTDbase
    {

        public TipoClienteOTD()
            : base(0, string.Empty)
        {
            _Set_campos<TipoClienteOTD>();
        }

        public TipoClienteOTD(long nId, string cDescripcion)
            : base(nId, cDescripcion)
        {
            _Set_campos<TipoClienteOTD>();
        }

        [CdgNetPersistenciaV3.Atributos.Campo("TIPOCLIENTE", AutoGenerado=true)]
        public long Id
        {
            get { return _nId;  }
            set { _nId = value; }
        }

        [CdgNetPersistenciaV3.Atributos.Campo("DESCRIP")]
        public string Descripcion
        {
            get { return _cDescripcion; }
            set { _cDescripcion = value; }
        }

    }


    public class TiposClientesADM : CdgNetPersistenciaV3.ClasesBases.ADMbase
    {

        public TiposClientesADM(CdgNetPersistenciaV3.ClasesBases.ConectorBase oConexion)
             : base(oConexion)
        {
            
            //establecemos la instancia de la clase de tabla a administrar
            _Set_tabla<TipoClienteOTD>();

        }

        public override List<object>  lAgregar(CdgNetPersistenciaV3.ClasesBases.OTDbase oOTDbase)
        {
 	        throw new NotImplementedException();
        }

        public override List<object>  lActualizar(CdgNetPersistenciaV3.ClasesBases.OTDbase oOTDbase)
        {
 	        throw new NotImplementedException();
        }

        public override List<object>  lEliminar(CdgNetPersistenciaV3.ClasesBases.OTDbase oOTDbase)
        {
 	        throw new NotImplementedException();
        }

        public override List<object>  lGet_elemento(CdgNetPersistenciaV3.ClasesBases.OTDbase oOTDbase)
        {
 	        throw new NotImplementedException();
        }

        public override List<object>  lGet_elementos(string cFiltroWhere, Dictionary<string,object> dicParametros)
        {
            return this.lGet_elementos<TipoClienteOTD>(cFiltroWhere, dicParametros);
        }
    }

}
