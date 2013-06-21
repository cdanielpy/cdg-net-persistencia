using System;
using System.Collections.Generic;

using System.Text;
using CdgNetPersistenciaV3.ClasesBases;
using System.Data;

namespace CdgNetPersistenciaV3.Extras
{
    /// <summary>
    /// Clase de administracion de parametros de configuracion desde SQLite
    /// </summary>
    public class SQLiteConfig
    {

        #region ATRIBUTOS DE LA CLASE

        public const string NOMBRE_CLASE = "SQLiteConfig";

        private const string __CADENA_CONEXION = "Data Source={0}; Version=3; Journal Mode=Off; Pooling=False;Max Pool Size=100;";

        private static string __NOMBRE_TABLA = "config";
        private const string __CAMPO_PARAMETROS = "parametro";
        private const string __CAMPO_VALORES = "valor";

        private const string __CONSULTA_PARAMETROS = "SELECT {0}, {1} FROM {2}";
        private const string __UPDATE_PARAMETRO = "UPDATE {0} SET {1} = '{2}' WHERE {3} = '{4}'";

        private ConectorBase __oConexion;
        private Dictionary<string, string> __dicParametros;


        #endregion


        /// <summary>
        /// Constructor de la clase
        /// </summary>
        /// <param name="cPathSQLite">Path absoluto al archivo SQLite</param>
        public SQLiteConfig(string cPathSQLite)
        {
            //creamos la instancia de la utileria
            __oConexion = new SQLiteUtiles(string.Format(__CADENA_CONEXION, cPathSQLite));

            __dicParametros = new Dictionary<string, string>();

            //llamamos al metodo de carga de parametros de configuracion
            __Cargar_parametros();

        }

        /// <summary>
        /// Constructor de la clase
        /// </summary>
        /// <param name="cPathSQLite">Path absoluto al archivo SQLite</param>
        /// <param name="cNombreTablaConfig">nombre de la tabla de configuraciones</param>
        public SQLiteConfig(string cPathSQLite, string cNombreTablaConfig)
        {
            //tomamos el nombre de la tabla
            __NOMBRE_TABLA = cNombreTablaConfig;

            //creamos la instancia de la utileria
            __oConexion = new SQLiteUtiles(string.Format(__CADENA_CONEXION, cPathSQLite));

            __dicParametros = new Dictionary<string, string>();

            //llamamos al metodo de carga de parametros de configuracion
            __Cargar_parametros();

        }

        /// <summary>
        /// Ejecuta la carga del diccionario de parametros de configuracion
        /// </summary>
        private void __Cargar_parametros()
        {
            string NOMBRE_METODO = NOMBRE_CLASE + ".__Cargar_parametros()";

            //abrimos la conexion al archivo
            List<object> lResultado = __oConexion.lConectar();

            //si se conecto sin problemas
            if ((int)lResultado[0] == 1)
            {
                //ejecutamos la consulta de recuperacion de parametros
                lResultado = __oConexion.lEjecutar_consulta(string.Format(__CONSULTA_PARAMETROS
                                                                            , __CAMPO_PARAMETROS
                                                                            , __CAMPO_VALORES
                                                                            , __NOMBRE_TABLA)
                                                            ,  new Dictionary<string,object>());

                //si se ejecuto correctamente
                if ((int)lResultado[0] == 1)
                {
                    DataTable dtResultado = (DataTable)lResultado[1];

                    //recorremos las filas de la tabla devuelta
                    foreach (DataRow dr in dtResultado.Rows)
                    {
                        //si el parametro no esta cargado aun
                        if (!__dicParametros.ContainsKey(dr[0].ToString()))
                            //cargamos el par al diccionario
                            __dicParametros.Add(dr[0].ToString(), dr[1].ToString());
                        else //si ya, lo actualizamos
                            __dicParametros[dr[0].ToString()] = dr[1].ToString();
                    }
                }

                //cerramos la conexion
                __oConexion.lDesconectar(NOMBRE_METODO);

            }

            //si hubo errores
            if((int)lResultado[0] != 1)
                //sino, mensaje de notificacion
                throw new Exception(NOMBRE_METODO + " -> " + lResultado[1].ToString());

        }

        /// <summary>
        /// Devuelve el valor del parametro solicitado
        /// </summary>
        /// <param name="cParametro">Cadena que representa el valor solicitado</param>
        /// <returns>Valor del parametro</returns>
        public string Get_valor(string cParametro)
        {
            return __dicParametros[cParametro];
        }

        /// <summary>
        /// Actualiza el valor de un parametro
        /// </summary>
        /// <param name="cParametroParam">Nombre del Parametro a actualizar</param>
        /// <param name="cValorNuevoParam">Nuevo valor del parametro</param>
        /// <returns>Lista de Resultados [int, object]</returns>
        public List<object> lSet_valor(string cParametroParam, string cValorNuevoParam)
        {

            string NOMBRE_METODO = NOMBRE_CLASE + ".lSet_valor()";

            //abrimos la conexion al archivo
            List<object> lResultado = __oConexion.lConectar();

            //si se conecto sin problemas
            if ((int)lResultado[0] == 1)
            {
                //ejecutamos la consulta de recuperacion de parametros
                lResultado = __oConexion.lEjecutar_sentencia(string.Format(__UPDATE_PARAMETRO
                                                                            , __NOMBRE_TABLA
                                                                            , __CAMPO_VALORES
                                                                            , cValorNuevoParam
                                                                            , __CAMPO_PARAMETROS
                                                                            , cParametroParam
                                                                            )
                                                             , new Dictionary<string,object>()
                                                             );

                //acualizamos el valor del parametro en el diccionario actual
                if (__dicParametros.ContainsKey(cParametroParam)) __dicParametros[cParametroParam] = cValorNuevoParam;

                //cerramos la conexion
                __oConexion.lDesconectar(NOMBRE_METODO);

            }

            //devolvemos el resultado del metodo
            return lResultado;

        }

    }
}
