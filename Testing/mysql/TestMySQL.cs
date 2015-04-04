using System;
using System.Collections.Generic;

namespace Testing.mysql
{
    /// <summary>
    /// Clase contenedora de metodos de prueba de utileria MySqlUtiles
    /// </summary>
    public class TestMySQL
    {

        /// <summary>
        /// Metodo de prueba de generacion de instancias de OTDs
        /// </summary>
        public static void TestOTDperformance()
        {
            System.Console.WriteLine("========================================================");
            System.Console.WriteLine("  Inicia el Test de Performace de OTDs");
            System.Console.WriteLine("========================================================");
            System.Console.WriteLine("");
            System.Console.WriteLine("");

            List<PersonaOTD> lista = new List<PersonaOTD>();

            System.Diagnostics.Stopwatch stopWatch = new System.Diagnostics.Stopwatch();
            stopWatch.Start();

            for (int i = 0; i < 1000; i++) lista.Add(new PersonaOTD(i, ""));

            stopWatch.Stop();

            // Get the elapsed time as a TimeSpan value.
            TimeSpan ts = stopWatch.Elapsed;

            System.Console.WriteLine("========================================================");
            System.Console.WriteLine(string.Format(" Tiempo transcurrido: {0}", ts));
            System.Console.WriteLine("========================================================");
            System.Console.ReadLine();

        }

        /// <summary>
        /// Metodo de prueba de generacion de instancias de ADMs
        /// </summary>
        public static void TestADMperformance()
        {
            System.Console.WriteLine("========================================================");
            System.Console.WriteLine("  Inicia el Test de Performace de ADMs");
            System.Console.WriteLine("========================================================");
            System.Console.WriteLine("");
            System.Console.WriteLine("");

            List<PersonasADM> listaADM = new List<PersonasADM>();

            System.Diagnostics.Stopwatch stopWatch = new System.Diagnostics.Stopwatch();
            stopWatch.Start();

            var oConexion = new CdgNetPersistenciaV3_5.SQLServerUtiles(".\\SQLEXPRESS", "TUTTI_FRUTTI", 120);
            System.Console.WriteLine(oConexion.aConectar());

            //prueba de instanciacion de ADM
            for (int i = 0; i < 1000; i++) listaADM.Add(new PersonasADM(oConexion));

            stopWatch.Stop();

            // Get the elapsed time as a TimeSpan value.
            TimeSpan ts = stopWatch.Elapsed;

            System.Console.WriteLine("========================================================");
            System.Console.WriteLine(string.Format(" Instancias recuperadas: {0}", listaADM.Count));
            System.Console.WriteLine(string.Format(" Tiempo transcurrido: {0}", ts));
            System.Console.WriteLine("========================================================");
            System.Console.ReadLine();

        }

        /// <summary>
        /// Metodo de pruebas de operaciones DML utilizando un ADM y varios OTDs
        /// </summary>
        public static void TestABMperformance()
        {
            System.Console.WriteLine("========================================================");
            System.Console.WriteLine("  Inicia el Test de Performace de ADMs");
            System.Console.WriteLine("========================================================");
            System.Console.WriteLine("");
            System.Console.WriteLine("");


            var oConexion = new CdgNetPersistenciaV3_5.MySqlUtiles("localhost", "test", "root", "Admin159", 60, 3306);
            oConexion.aConectar();

            oConexion.aIniciar_transaccion();

            //creamos una instancia del administrador de la tabla
            var oPersonADM = new PersonasADM(oConexion);

            //esta parte se puede hacer de dos modos

            //SQL Nativo
            //var cSelect = string.Format("SELECT * FROM {0} LIMIT 1000", oPersonADM.NombreTabla);
            //var aResultado = oConexion.aEjecutar_consulta(cSelect);

            //Utilizando el metodo correspondiente
            var aResultado = oPersonADM.aGet_elementos(string.Empty, 1000);

            //si se ejecuto correctamente
            if ((int)aResultado[0] == 1)
            {
                //Si se ejecuto mediante un SQL Nativo

                //tomamos las filas de la tabla devuelta y las convertimos a OTDs
                //List<PersonaOTD> lPersonas = oPersonADM.lSet_registros<PersonaOTD>((aResultado[1] as System.Data.DataTable).Rows);

                //Sino solo tomamos la lista de instancias devueltas
                List<PersonaOTD> lPersonas = (List<PersonaOTD>) aResultado[1];

                /*************************************************************
                 para realizar el test de inserciones se puede comentar esta parte
                 *************************************************************/
                foreach (PersonaOTD oOTD in lPersonas)
                {
                    oOTD.Descripcion = oOTD.Descripcion.Substring(0, 6) + string.Format("{0:HHmmss}", DateTime.Now);
                    oOTD.Activo = !oOTD.Activo;
                }

                /*************************************************************/

                System.Diagnostics.Stopwatch stopWatch = new System.Diagnostics.Stopwatch();
                stopWatch.Start();

                //recorremos la lista de instancias
                foreach (PersonaOTD oOTD in lPersonas)
                {
                    //TEST DE ACTUALIZACIONES
                    aResultado = oPersonADM.aActualizar(oOTD);

                    //TEST DE ELIMINACIONES - Primero hay que insertar los registros
                    //aResultado = oPersonADM.aEliminar(oOTD);

                    //al primer error salimos
                    if ((int)aResultado[0] != 1) Console.WriteLine(aResultado[1].ToString());

                }

                stopWatch.Stop();

                oConexion.aConfirmar_transaccion();

                // Get the elapsed time as a TimeSpan value.
                TimeSpan ts = stopWatch.Elapsed;

                System.Console.WriteLine("========================================================");
                System.Console.WriteLine(string.Format(" Tiempo transcurrido: {0}", ts));
                System.Console.WriteLine("========================================================");
                System.Console.ReadLine();

            }
            else
            {
                //una lista para las nuevas instancias
                List<PersonaOTD> lPersonas = new List<PersonaOTD>();

                /*************************************************************
                 para realizar el test de inserciones se puede comentar esta parte
                 *************************************************************/
                for (int i = 0; i < 1000; i++)
                {
                    var oOTD = new PersonaOTD();
                    oOTD.Descripcion = string.Format("{0:HHmmss}", DateTime.Now);
                    oOTD.Activo = i % 2 == 0;

                    lPersonas.Add(oOTD);
                }

                /*************************************************************/

                System.Diagnostics.Stopwatch stopWatch = new System.Diagnostics.Stopwatch();
                stopWatch.Start();

                //recorremos la lista de instancias
                foreach (PersonaOTD oOTD in lPersonas)
                {
                    //TEST DE INSERCIONES
                    aResultado = oPersonADM.aAgregar(oOTD);

                    //al primer error salimos
                    if ((int)aResultado[0] != 1)
                    {
                        Console.WriteLine(aResultado[1].ToString());
                        break;
                    }
                }

                stopWatch.Stop();

                oConexion.aConfirmar_transaccion();

                // Get the elapsed time as a TimeSpan value.
                TimeSpan ts = stopWatch.Elapsed;

                System.Console.WriteLine("========================================================");
                System.Console.WriteLine(string.Format(" Tiempo transcurrido: {0}", ts));
                System.Console.WriteLine("========================================================");
                System.Console.ReadLine();
            }



        }

    }
}
