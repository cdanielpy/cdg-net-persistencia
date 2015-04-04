using System;
using System.Collections.Generic;
using CdgNetPersistenciaV3_5.ClasesBases;

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
            //Testing.sqlserver.TestSQLServer.TestABMperformance();

            //no olvidar cambiar el tipo de SGBD de la clase PersonaOTD
            Testing.mysql.TestMySQL.TestABMperformance();


            //resultados de testing sql server
            //1000 inserciones promedio .7760401 sin transacciones
            //1000 inserciones promedio .3345617 con transacciones

            //1000 actualizaciones promedio .921586 sin transacciones
            //1000 actualizaciones promedio .4017576 con transacciones

            //1000 eliminaciones promedio .86572593 sin transacciones
            //1000 eliminaciones promedio .13995927 con transacciones
            
        }
    }

}
