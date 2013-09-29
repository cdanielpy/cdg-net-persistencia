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
            //PC => HP Pavilion DV7 6187cl
            //http://bit.ly/1dPRqdd

            //TestSQLServer.TestOTDperformance(); //1000 instancias promedio .0003850

            //TestSQLServer.TestADMperformance(); //1000 instancias promedio .400

            TestSQLServer.TestABMperformance();
            //1000 inserciones promedio .7760401 sin transacciones
            //1000 inserciones promedio .3345617 con transacciones

            //1000 actualizaciones promedio .921586 sin transacciones
            //1000 actualizaciones promedio .4017576 con transacciones

            //1000 eliminaciones promedio .86572593 sin transacciones
            //1000 eliminaciones promedio .13995927 con transacciones
            
            

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

            List<PersonOTD> lista = new List<PersonOTD>();

            System.Diagnostics.Stopwatch stopWatch = new System.Diagnostics.Stopwatch();
            stopWatch.Start();

            for (int i = 0; i < 1000; i++) lista.Add(new PersonOTD(i, ""));

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

            List<PersonADM> lista = new List<PersonADM>();
            List<PersonOTD> listaOTD = new List<PersonOTD>();

            System.Diagnostics.Stopwatch stopWatch = new System.Diagnostics.Stopwatch();
            stopWatch.Start();

            var oConexion = new CdgNetPersistenciaV3.SQLServerUtiles(".\\SQLEXPRESS", "AdventureWorks2012", 120);
            oConexion.lConectar();

            //utilizando instanciacion normal
            for (int i = 0; i < 1000; i++) lista.Add(new PersonADM(oConexion));
            
            /*
            //utilizando un singleton
            for (int i = 0; i < 10; i++) lista.Add(TiposClientesADM.Instancia(oConexion));

            foreach (TiposClientesADM oADM in lista)
            {
                var lResultado = oADM.lGet_elementos(string.Empty, new Dictionary<string, object>());
                if((int)lResultado[0] == 1)
                    listaOTD.AddRange(lResultado[1] as List<PersonOTD>);
            }*/

            stopWatch.Stop();

            // Get the elapsed time as a TimeSpan value.
            TimeSpan ts = stopWatch.Elapsed;

            System.Console.WriteLine("========================================================");
            System.Console.WriteLine(string.Format(" Instancias recuperadas: {0}", listaOTD.Count()));
            if(listaOTD.Count() > 0)
                System.Console.WriteLine(string.Format(" Instancia ejemplo: {0}", listaOTD[822]));
            System.Console.WriteLine(string.Format(" Tiempo transcurrido: {0}", ts));
            System.Console.WriteLine("========================================================");
            System.Console.ReadLine();

        }

        public static void TestABMperformance(){
            System.Console.WriteLine("========================================================");
            System.Console.WriteLine("  Inicia el Test de Performace de ADMs");
            System.Console.WriteLine("========================================================");
            System.Console.WriteLine("");
            System.Console.WriteLine("");
            

            var oConexion = new CdgNetPersistenciaV3.SQLServerUtiles(".\\SQLEXPRESS", "AdventureWorks2012", 120);
            oConexion.lConectar();
            
            oConexion.lIniciar_transaccion();

            //creamos una instancia del administrador de la tabla
            var oPersonADM = new PersonADM(oConexion);
            var oPersonDestinoADM = new PersonTestADM(oConexion);

            //recuperamos los primeros N registros
            var cSelect = string.Format("SELECT TOP 1000 * FROM {0}", oPersonADM.NombreTabla);
            var lResultado = oConexion.lEjecutar_consulta(cSelect, new Dictionary<string,object>());

            //si se ejecuto correctamente
            if((int)lResultado[0] == 1){
                //tomamos las filas de la tabla devuelta y las convertimos a OTDs
                List<PersonTestOTD> lPersonas = oPersonDestinoADM.lSet_registros<PersonTestOTD>((lResultado[1] as System.Data.DataTable).Rows);

                /*************************************************************
                 para realizar el test de inserciones se puede comentar esta parte
                 *************************************************************/
                var cHora = string.Format("{0:HHmmss}", DateTime.Now);
                foreach (PersonTestOTD oOTD in lPersonas) oOTD.Suffix = cHora;

                /*************************************************************/

                System.Diagnostics.Stopwatch stopWatch = new System.Diagnostics.Stopwatch();
                stopWatch.Start();

                //recorremos la lista de instancias
                foreach(PersonTestOTD oOTD in lPersonas){
                    //las insertamos en el tabla de pruebas

                    //TEST DE INSERCIONES 
                    lResultado = oPersonDestinoADM.lAgregar(oOTD);

                    //TEST DE ACTUALIZACIONES
                    //lResultado = oPersonDestinoADM.lActualizar(oOTD);

                    //TEST DE ELIMINACIONES - Primero hay que insertar los mismos registros
                    //lResultado = oPersonDestinoADM.lEliminar(oOTD);

                    //al primer error salimos
                    if ((int)lResultado[0] != 1) Console.WriteLine(lResultado[1].ToString());

                }
                
                
                stopWatch.Stop();

                oConexion.lConfirmar_transaccion();

                // Get the elapsed time as a TimeSpan value.
                TimeSpan ts = stopWatch.Elapsed;

                System.Console.WriteLine("========================================================");
                System.Console.WriteLine(string.Format(" Tiempo transcurrido: {0}", ts));
                System.Console.WriteLine("========================================================");
                System.Console.ReadLine();


            }


            

        }

    }

    public class PersonADM : CdgNetPersistenciaV3.ClasesBases.ADMbase
    {

        private static PersonADM __oInstancia;

        public PersonADM(CdgNetPersistenciaV3.ClasesBases.ConectorBase oConexion)
             : base(oConexion, new PersonOTD())
        {
        }

        public static PersonADM Instancia(CdgNetPersistenciaV3.ClasesBases.ConectorBase oConexion)
        {
                if (__oInstancia == null)
                    __oInstancia = new PersonADM(oConexion);
                return __oInstancia;
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
            return this.lGet_elementos<PersonOTD>(cFiltroWhere, dicParametros);
        }
    }

    [CdgNetPersistenciaV3.Atributos.Tabla("PERSON.PERSON", CdgNetPersistenciaV3.Atributos.Tabla.SGBD.SQL_SERVER)]
    public class PersonOTD : CdgNetPersistenciaV3.ClasesBases.OTDbase
    {

        public PersonOTD()
            : base(0, string.Empty)
        {
        }

        public PersonOTD(long nId, string cNombreCompleto)
            : base(nId, cNombreCompleto)
        {
        }

        [CdgNetPersistenciaV3.Atributos.Campo("BusinessEntityID", Identificador = true)]
        public override long Id
        {
            get { return _nId;  }
            set { _nId = value; }
        }

        [CdgNetPersistenciaV3.Atributos.Campo("FirstName", Nulable = false)]
        public string FirstName
        {
            get { return _cDescripcion; }
            set { _cDescripcion = value; }
        }

        [CdgNetPersistenciaV3.Atributos.Campo("Title")]
        public string Title
        {
            get; set;
        }

        [CdgNetPersistenciaV3.Atributos.Campo("Suffix")]
        public string Suffix
        {
            get; set;
        }

    }


    public class PersonTestADM : CdgNetPersistenciaV3.ClasesBases.ADMbase
    {

        public PersonTestADM(CdgNetPersistenciaV3.ClasesBases.ConectorBase oConexion)
            : base(oConexion, new PersonTestOTD())
        {
        }

        public override List<object> lAgregar(CdgNetPersistenciaV3.ClasesBases.OTDbase oOTDbase)
        {
            return lAgregar<PersonTestOTD>(oOTDbase);
        }

        public override List<object> lActualizar(CdgNetPersistenciaV3.ClasesBases.OTDbase oOTDbase)
        {
            return lActualizar<PersonTestOTD>(oOTDbase);
        }

        public override List<object> lEliminar(CdgNetPersistenciaV3.ClasesBases.OTDbase oOTDbase)
        {
            return lEliminar<PersonTestOTD>(oOTDbase);
        }

        public override List<object> lGet_elemento(CdgNetPersistenciaV3.ClasesBases.OTDbase oOTDbase)
        {
            throw new NotImplementedException();
        }

        public override List<object> lGet_elementos(string cFiltroWhere, Dictionary<string, object> dicParametros)
        {
            return this.lGet_elementos<PersonTestOTD>(cFiltroWhere, dicParametros);
        }
    }

    [CdgNetPersistenciaV3.Atributos.Tabla("PERSON.PERSON_TEST", CdgNetPersistenciaV3.Atributos.Tabla.SGBD.SQL_SERVER)]
    public class PersonTestOTD : CdgNetPersistenciaV3.ClasesBases.OTDbase
    {

        public PersonTestOTD()
            : base(0, string.Empty)
        {
        }

        public PersonTestOTD(long nId, string cNombreCompleto)
            : base(nId, cNombreCompleto)
        {
        }

        [CdgNetPersistenciaV3.Atributos.Campo("BusinessEntityID", Identificador=true)]
        public override long Id
        {
            get { return _nId; }
            set { _nId = value; }
        }

        [CdgNetPersistenciaV3.Atributos.Campo("FirstName", Nulable = false)]
        public string FirstName
        {
            get { return _cDescripcion; }
            set { _cDescripcion = value; }
        }

        [CdgNetPersistenciaV3.Atributos.Campo("Title")]
        public string Title
        {
            get;
            set;
        }

        [CdgNetPersistenciaV3.Atributos.Campo("Suffix")]
        public string Suffix
        {
            get;
            set;
        }

    }


}
