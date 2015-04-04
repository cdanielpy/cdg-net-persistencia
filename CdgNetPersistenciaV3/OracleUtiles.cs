using System;
using System.Collections.Generic;

using System.Text;
using System.Data.OracleClient;
using CdgNetPersistenciaV3_5.ClasesBases;
using System.Data;

namespace CdgNetPersistenciaV3_5
{
    /// <summary>
    /// Utileria para interaccion con ConectorBase de datos Oracle
    /// </summary>
    public class OracleUtiles : ConectorBase
    {

        #region ATRIBUTOS DE LA CLASE

        /// <summary>
        /// Nombre de la clase
        /// </summary>
        public const string NOMBRE_CLASE = "OracleUtiles";

        /// <summary>
        /// Cadena de Conexion
        /// </summary>
        private const string CADENA_CONEXION_ORA = "Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST={0})(PORT={1}))(CONNECT_DATA=(SERVICE_NAME="
                                                            + "{2})));User Id={3};Password={4};";

        //"SERVER=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST={0})(PORT={1}))(CONNECT_DATA=(SERVICE_NAME={2})));uid={3};pwd={4};"

        /// <summary>
        /// Caracter de marca de parametros de comando PL-SQL
        /// </summary>
        public new static char MARCADOR_PARAMETRO = ':';

        private OracleConnectionStringBuilder __oCadenaConexion;
        private OracleConnection __oConexion;
        private OracleTransaction __oTransaccionSQL;

        #endregion


        #region TIPOS_DE_DATOS



        #endregion


        #region CONSTRUCTORES DE LA CLASE

        /// <summary>
        /// Constructor de la clase
        /// </summary>
        /// <param name="cCadenaConexionParam">Cadena de conexion personalizada</param>
        public OracleUtiles(string cCadenaConexionParam)
            : base(cCadenaConexionParam)
        {
            //tomamos la cadena de conexion
            __oCadenaConexion = new OracleConnectionStringBuilder(cCadenaConexionParam);

        }

        /// <summary>
        /// Constructor de la clase
        /// </summary>
        /// <param name="cServidorParam">Nombre o IP del servidor de base de datos</param>
        /// <param name="cUsuarioParam">ID de Usuario</param>
        /// <param name="cContrasenaParam">Contrasena</param>
        /// <param name="cServicioParam">Nombre del servicio de base de datos</param>
        /// <param name="nTiempoComandoParam">Tiempo de espera a ejecucion de comandos</param>
        /// <param name="nPuertoParam">Puerto de escucha del servicio</param>
        public OracleUtiles(string cServidorParam, string cUsuarioParam, string cContrasenaParam
                                , string cServicioParam, int nPuertoParam, int nTiempoComandoParam
                                )
            : base(cUsuarioParam, cContrasenaParam, cServidorParam, string.Empty, nPuertoParam, cServicioParam)
        {

            //formateamos la cadena de conexion
            __oCadenaConexion = new OracleConnectionStringBuilder(string.Format(CADENA_CONEXION_ORA
                                                                            , _cServidor
                                                                            , _nPuerto
                                                                            , _cServicio
                                                                            , _cUsuario
                                                                            , _cContrasena)
                                                                );

            nTiempoComandos = nTiempoComandoParam;

        }

        /// <summary>
        /// Constructor de la clase
        /// </summary>
        /// <param name="cServidorParam">Nombre o IP del servidor de base de datos</param>
        /// <param name="cUsuarioParam">ID de Usuario</param>
        /// <param name="cContrasenaParam">Contrasena</param>
        /// <param name="cServicioParam">Nombre del servicio de base de datos</param>
        /// <param name="nTiempoComandoParam">Tiempo de espera a ejecucion de comandos</param>
        /// <param name="nPuertoParam">Puerto de escucha del servicio</param>
        /// <param name="cCadenaConexionParam">Cadena de Conexion personalizada a Implementar</param>
        public OracleUtiles(string cServidorParam, string cUsuarioParam, string cContrasenaParam
                                , string cServicioParam, int nPuertoParam, int nTiempoComandoParam
                                , string cCadenaConexionParam
                                )
            : base(cUsuarioParam, cContrasenaParam, cServidorParam, string.Empty, nPuertoParam, cServicioParam)
        {

            //formateamos la cadena de conexion
            __oCadenaConexion = new OracleConnectionStringBuilder(string.Format(cCadenaConexionParam
                                                                            , _cServidor
                                                                            , _nPuerto
                                                                            , _cServicio
                                                                            , _cUsuario
                                                                            , _cContrasena)
                                                                );

            nTiempoComandos = nTiempoComandoParam;

        }

        #endregion


        #region Miembros de ConectorBase

        /// <summary>
        /// Abre la conexion a la base de datos
        /// </summary>
        /// <returns>Arreglo de Resultados[int, object]</returns>
        public override object[] aConectar()
        {
            string NOMBRE_METODO = NOMBRE_CLASE + ".lConectar()";
            object[] aResultado = new object[] { 0, NOMBRE_METODO + " No Ejecutado." };

            //intentamos abrir la conexion a la base de datos
            try
            {
                __oConexion = new OracleConnection(__oCadenaConexion.ConnectionString);

                __oConexion.Open();

                aResultado = new object[] { 1, "Ok" };

            }
            catch (Exception ex)
            {
                //en caso de error
                aResultado = new object[] { -1, NOMBRE_METODO + ": " + ex.Message };
            }

            //devolvemos el resultado del metodo
            return aResultado;
        }

        /// <summary>
        /// Cierra la conexion actual a la base de datos
        /// </summary>
        /// <returns>Arreglo de Resultados[int, object]</returns>
        public override object[] aDesconectar(object sender)
        {
            string NOMBRE_METODO = NOMBRE_CLASE + ".lDesconectar()";
            object[] aResultado = new object[] { 0, NOMBRE_METODO + " No Ejecutado." };

            try
            {
                //si la conexion esta abierta, la intentamos cerrar la conexion a la base de datos
                if (__oConexion.State == System.Data.ConnectionState.Open) Conexion.Close();
                GC.Collect();

                aResultado = new object[] { 1, "Ok" };
            }
            catch (Exception ex)
            {
                //en caso de error
                aResultado = new object[] { -1, NOMBRE_METODO + ": " + ex.Message };
            }

            //devolvemos el resultado del metodo
            return aResultado;
        }

        /// <summary>
        /// Ejecuta la sentencia parametro
        /// </summary>
        /// <param name="cSentenciaSQL">Sentencia a Ejecutar</param>
        /// <param name="dicParametros">Diccionario de parametros a combinar con la sentencia</param>
        /// <returns>Arreglo de Resultados[int, object]</returns>
        public override object[] aEjecutar_sentencia(string cSentenciaSQL, Dictionary<string, object> dicParametros)
        {
            string NOMBRE_METODO = NOMBRE_CLASE + ".lEjecutar_sentencia()";
            object[] aResultado = new object[] { 0, NOMBRE_METODO + " No Ejecutado." };

            try
            {
                //recorremos el diccionario de parametros
                foreach (string cNombre in dicParametros.Keys)
                {
                    //si es nulo el valor
                    if (dicParametros[cNombre] == null)
                        //lo reemplazamos en la sentencia y lo eliminamos del diccionario de parametros
                        if (cSentenciaSQL.IndexOf(MARCADOR_PARAMETRO + cNombre + ",") > -1)
                            cSentenciaSQL = cSentenciaSQL.Replace(MARCADOR_PARAMETRO + cNombre + ",", "NULL,");
                        else if (cSentenciaSQL.IndexOf(MARCADOR_PARAMETRO + cNombre + " ") > -1)
                            cSentenciaSQL = cSentenciaSQL.Replace(MARCADOR_PARAMETRO + cNombre + " ", "NULL ");
                        else if (cSentenciaSQL.IndexOf(MARCADOR_PARAMETRO + cNombre) > -1)
                            cSentenciaSQL = cSentenciaSQL.Replace(MARCADOR_PARAMETRO + cNombre, "NULL");
                }

                //creamos una instancia de comando
                OracleCommand oComandoSQL;

                //si hay una transaccion activa
                if (__oTransaccionSQL != null)
                {
                    //asignamos el comando como parte de la misma
                    oComandoSQL = __oTransaccionSQL.Connection.CreateCommand();
                    oComandoSQL.CommandText = cSentenciaSQL;
                    oComandoSQL.Transaction = __oTransaccionSQL;
                }
                else
                {
                    //sino, la conexion actual de la clase
                    oComandoSQL = new OracleCommand(cSentenciaSQL);
                    oComandoSQL.Connection = this.Conexion;
                }

                //preparamos la sentencia a ejecutar
                oComandoSQL.Prepare();

                //configuramos el comando
                oComandoSQL.CommandTimeout = nTiempoComandos;

                //recorremos el diccionario de parametros
                foreach (string cNombre in dicParametros.Keys)
                {
                    //si el valor NO es nulo
                    if (dicParametros[cNombre] != null)
                        //lo agregamos al comando los parametros y sus valores
                        oComandoSQL.Parameters.AddWithValue(cNombre, dicParametros[cNombre]);
                }

                _Mostrar_SQL(oComandoSQL.CommandText);

                //ejecutamos el comando
                oComandoSQL.ExecuteNonQuery();

                //establecemos el resultado
                aResultado = new object[] { 1, "Ok" };

            }
            catch (Exception ex)
            {
                //en caso de error
                aResultado = new object[] { -1, NOMBRE_METODO + ": " + ex.Message + (char)13 + "Sentencia -> " + cSentenciaSQL };
            }

            //devolvemos el resultado del metodo
            return aResultado;

        }

        /// <summary>
        /// Ejecuta la consulta parametro y devuelve la lista de resultados
        /// </summary>
        /// <param name="cConsultaSQL">Consulta a ejecutar</param>
        /// <param name="dicParametros">Diccionario de parametros a combinar con la consulta</param>
        /// <returns>Arreglo de Resultados[int, object]</returns>
        public override object[] aEjecutar_consulta(string cConsultaSQL, Dictionary<string, object> dicParametros)
        {
            string NOMBRE_METODO = NOMBRE_CLASE + ".lEjecutar_consulta()";
            object[] aResultado = new object[] { 0, NOMBRE_METODO + " No Ejecutado." };

            try
            {
                //recorremos el diccionario de parametros
                foreach (string cNombre in dicParametros.Keys)
                {
                    //si es nulo el valor
                    if (dicParametros[cNombre] == null)
                        //lo reemplazamos en la sentencia y lo eliminamos del diccionario de parametros
                        if (cConsultaSQL.IndexOf(MARCADOR_PARAMETRO + cNombre + ",") > -1)
                            cConsultaSQL = cConsultaSQL.Replace(MARCADOR_PARAMETRO + cNombre + ",", "NULL,");
                        else if (cConsultaSQL.IndexOf(MARCADOR_PARAMETRO + cNombre + " ") > -1)
                            cConsultaSQL = cConsultaSQL.Replace(MARCADOR_PARAMETRO + cNombre + " ", "NULL ");
                        else if (cConsultaSQL.IndexOf(MARCADOR_PARAMETRO + cNombre) > -1)
                            cConsultaSQL = cConsultaSQL.Replace(MARCADOR_PARAMETRO + cNombre, "NULL");
                }

                //creamos una instancia de comando
                OracleCommand oComandoSQL;

                //si hay una transaccion activa
                if (__oTransaccionSQL != null)
                {
                    //asignamos el comando como parte de la misma
                    oComandoSQL = __oTransaccionSQL.Connection.CreateCommand();
                    oComandoSQL.CommandText = cConsultaSQL;
                    oComandoSQL.Transaction = __oTransaccionSQL;
                }
                else
                {
                    //sino, la conexion actual de la clase
                    oComandoSQL = new OracleCommand(cConsultaSQL);
                    oComandoSQL.Connection = this.Conexion;
                }

                //preparamos la sentencia a ejecutar
                oComandoSQL.Prepare();

                //configuramos el comando
                oComandoSQL.CommandTimeout = nTiempoComandos;

                //si hay una transaccion activa, asignamos el comando como parte de la misma
                if (__oTransaccionSQL != null) oComandoSQL.Transaction = __oTransaccionSQL;

                //recorremos el diccionario de parametros
                foreach (string cNombre in dicParametros.Keys)
                {
                    //si el valor NO es nulo
                    if (dicParametros[cNombre] != null)
                        //lo agregamos al comando los parametros y sus valores
                        oComandoSQL.Parameters.AddWithValue(cNombre, dicParametros[cNombre]);
                }

                OracleDataAdapter daAdaptador = new OracleDataAdapter(oComandoSQL);
                DataTable dtTabla = new DataTable();

                _Mostrar_SQL(oComandoSQL.CommandText);

                //el resultado a la tabla
                daAdaptador.Fill(dtTabla);

                //si hay filas devueltas
                if (dtTabla.Rows.Count > 0) aResultado = new object[] { 1, dtTabla, daAdaptador };
                else aResultado = new object[] { 0, ConectorBase.ERROR_NO_HAY_FILAS, oComandoSQL.CommandText };

            }
            catch (Exception ex)
            {
                //en caso de error
                aResultado = new object[] { -1, NOMBRE_METODO + ": " + ex.Message + (char)13 + "Sentencia -> " + cConsultaSQL };
            }

            //devolvemos el resultado del metodo
            return aResultado;

        }

        /// <summary>
        /// Ejecuta la consulta parametro y devuelve el valor del primer campo
        /// de la primera fila recuperada
        /// </summary>
        /// <param name="cConsultaSQL">Consulta a ejecutar</param>
        /// <param name="dicParametros">Diccionario de parametros a combinar con la consulta</param>
        /// <returns>Arreglo de Resultados[int, object]</returns>
        public override object[] aEjecutar_escalar(string cConsultaSQL, Dictionary<string, object> dicParametros)
        {
            string NOMBRE_METODO = NOMBRE_CLASE + ".lEjecutar_escalar()";

            object[] aResultado = new object[] { 0, NOMBRE_METODO + " No Ejecutado." };

            try
            {
                //recorremos el diccionario de parametros
                foreach (string cNombre in dicParametros.Keys)
                {
                    //si es nulo el valor
                    if (dicParametros[cNombre] == null)
                        //lo reemplazamos en la sentencia y lo eliminamos del diccionario de parametros
                        if (cConsultaSQL.IndexOf(MARCADOR_PARAMETRO + cNombre + ",") > -1)
                            cConsultaSQL = cConsultaSQL.Replace(MARCADOR_PARAMETRO + cNombre + ",", "NULL,");
                        else if (cConsultaSQL.IndexOf(MARCADOR_PARAMETRO + cNombre + " ") > -1)
                            cConsultaSQL = cConsultaSQL.Replace(MARCADOR_PARAMETRO + cNombre + " ", "NULL ");
                        else if (cConsultaSQL.IndexOf(MARCADOR_PARAMETRO + cNombre) > -1)
                            cConsultaSQL = cConsultaSQL.Replace(MARCADOR_PARAMETRO + cNombre, "NULL");
                }

                //creamos una instancia de comando
                OracleCommand oComandoSQL;

                //si hay una transaccion activa
                if (__oTransaccionSQL != null)
                {
                    //asignamos el comando como parte de la misma
                    oComandoSQL = __oTransaccionSQL.Connection.CreateCommand();
                    oComandoSQL.CommandText = cConsultaSQL;
                    oComandoSQL.Transaction = __oTransaccionSQL;
                }
                else
                {
                    //sino, la conexion actual de la clase
                    oComandoSQL = new OracleCommand(cConsultaSQL);
                    oComandoSQL.Connection = this.Conexion;
                }

                //preparamos la sentencia a ejecutar
                oComandoSQL.Prepare();

                //recorremos el diccionario de parametros
                foreach (string cNombre in dicParametros.Keys)
                {
                    //si el valor NO es nulo
                    if (dicParametros[cNombre] != null)
                        //lo agregamos al comando los parametros y sus valores
                        oComandoSQL.Parameters.AddWithValue(cNombre, dicParametros[cNombre]);
                }

                _Mostrar_SQL(oComandoSQL.CommandText);

                //ejecutamos la consulta
                object oValor = oComandoSQL.ExecuteScalar();

                //establecemos el resultado del metodo
                aResultado = new object[] { 1, oValor };

            }
            catch (Exception ex)
            {
                //en caso de error
                aResultado = new object[] { -1, NOMBRE_METODO + ": " + ex.Message };
            }

            //devolvemos el resultado del metodo
            return aResultado;

        }

        /// <summary>
        /// Ejecuta el procedimiento almacenado y devuelve una lista de resultado
        /// </summary>
        /// <param name="cProcedimiento">Procedimiento Almacenado a ejecutar</param>
        /// <param name="dicParametros">Diccionario de claves y valores parametros</param>
        /// <returns>Arreglo de Resultados [int, object]</returns>
        public override object[] aEjecutar_procedimiento(string cProcedimiento, Dictionary<string, object> dicParametros)
        {
            string NOMBRE_METODO = NOMBRE_CLASE + ".lEjecutar_procedimiento()";
            object[] aResultado = new object[] { 0, NOMBRE_METODO + " No Ejecutado." };

            try
            {
                //recorremos el diccionario de parametros
                foreach (string cNombre in dicParametros.Keys)
                {
                    //si es nulo el valor
                    if (dicParametros[cNombre] == null)
                        //lo reemplazamos en la sentencia y lo eliminamos del diccionario de parametros
                        if (cProcedimiento.IndexOf(MARCADOR_PARAMETRO + cNombre + ",") > -1)
                            cProcedimiento = cProcedimiento.Replace(MARCADOR_PARAMETRO + cNombre + ",", "NULL,");
                        else if (cProcedimiento.IndexOf(MARCADOR_PARAMETRO + cNombre + " ") > -1)
                            cProcedimiento = cProcedimiento.Replace(MARCADOR_PARAMETRO + cNombre + " ", "NULL ");
                        else if (cProcedimiento.IndexOf(MARCADOR_PARAMETRO + cNombre) > -1)
                            cProcedimiento = cProcedimiento.Replace(MARCADOR_PARAMETRO + cNombre, "NULL");
                }


                //creamos una instancia de comando
                OracleCommand oComandoSQL;

                //si hay una transaccion activa
                if (__oTransaccionSQL != null)
                {
                    //asignamos el comando como parte de la misma
                    oComandoSQL = __oTransaccionSQL.Connection.CreateCommand();
                    oComandoSQL.CommandText = cProcedimiento;
                    oComandoSQL.Transaction = __oTransaccionSQL;
                }
                else
                {
                    //sino, la conexion actual de la clase
                    oComandoSQL = new OracleCommand(cProcedimiento);
                    oComandoSQL.Connection = this.Conexion;
                }

                bool bDevuelveDatos = false;

                //configuramos el comando
                oComandoSQL.CommandTimeout = nTiempoComandos;
                oComandoSQL.CommandType = CommandType.StoredProcedure;

                //preparamos la sentencia a ejecutar
                oComandoSQL.Prepare();

                //recorremos el diccionario de parametros
                foreach (string cNombre in dicParametros.Keys)
                {
                    //si el valor NO es nulo
                    if (dicParametros[cNombre] != null)
                        //lo agregamos al comando los parametros y sus valores
                        oComandoSQL.Parameters.AddWithValue(cNombre, dicParametros[cNombre]);
                }

                _Mostrar_SQL(oComandoSQL.CommandText);

                //si devuelve datos
                if (bDevuelveDatos)
                {
                    //asignamos a un adaptador de datos el comando
                    OracleDataAdapter daDataAdapter = new OracleDataAdapter(oComandoSQL);
                    DataTable dtTabla = new DataTable();

                    //cargamos el resultado del comando a la tabla
                    daDataAdapter.Fill(dtTabla);

                    //establecemos el resultado del metodo
                    aResultado = new object[] { 1, dtTabla };
                }
                else
                {
                    //ejecutamos el comando
                    object oValor = oComandoSQL.ExecuteNonQuery();

                    //establecemos el resultado del metodo
                    aResultado = new object[] { 1, "Ok" };
                }
            }
            catch (Exception ex)
            {
                //en caso de error
                aResultado = new object[] { -1, NOMBRE_METODO + ": " + ex.Message + (char)13 + "Sentencia -> " + cProcedimiento };
            }

            //devolvemos el resultado del metodo
            return aResultado;
        }

        /// <summary>
        /// No implementado
        /// </summary>
        /// <returns>Lista de resultado</returns>
        public override void Set_autocommit(bool bParam)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Ejecuta el comando de apertura de una Transaccion
        /// </summary>
        /// <returns>Lista de resultado</returns>
        public override object[] aIniciar_transaccion()
        {
            string NOMBRE_METODO = NOMBRE_CLASE + ".lIniciar_transaccion()";
            object[] aResultado = new object[] { 0, NOMBRE_METODO + " No Ejecutado." };

            try
            {
                //si hay una transaccion activa
                if (__oTransaccionSQL != null)
                    //generamos un error
                    throw new Exception("Hay una transaccion pendiente! \n Confirmela o reviertala antes de iniciar otra...");

                //iniciamos una transaccion
                __oTransaccionSQL = this.Conexion.BeginTransaction();

                aResultado = new object[] { 1, "Ok" };
            }
            catch (Exception ex)
            {
                //en caso de error
                aResultado = new object[] { -1, NOMBRE_METODO + ": " + ex.Message };
            }

            //devolvemos el resultado del metodo
            return aResultado;
        }

        /// <summary>
        /// Confirma la ultima Transaccion iniciada
        /// </summary>
        /// <returns>Lista de resultado</returns>
        public override object[] aConfirmar_transaccion()
        {
            string NOMBRE_METODO = NOMBRE_CLASE + ".lConfirmar_transaccion()";
            object[] aResultado = new object[] { 0, NOMBRE_METODO + " No Ejecutado." };

            try
            {
                //confirmamos la transaccion
                __oTransaccionSQL.Commit();

                //la liberamos
                __oTransaccionSQL.Dispose();
                __oTransaccionSQL = null;

                aResultado = new object[] { 1, "Ok" };
            }
            catch (Exception ex)
            {
                //en caso de error
                aResultado = new object[] { -1, NOMBRE_METODO + ": " + ex.Message };
            }

            //devolvemos el resultado del metodo
            return aResultado;
        }

        /// <summary>
        /// Revierte la ultima Transaccion Iniciada
        /// </summary>
        /// <returns>Lista de resultado</returns>
        public override object[] aRevertir_transaccion()
        {
            string NOMBRE_METODO = NOMBRE_CLASE + ".lRevertir_transaccion()";
            object[] aResultado = new object[] { 0, NOMBRE_METODO + " No Ejecutado." };

            try
            {
                //revertimos la transaccion
                __oTransaccionSQL.Rollback();

                //la liberamos
                __oTransaccionSQL.Dispose();
                __oTransaccionSQL = null;

                aResultado = new object[] { 1, "Ok" };
            }
            catch (Exception ex)
            {
                //en caso de error
                aResultado = new object[] { -1, NOMBRE_METODO + ": " + ex.Message };
            }

            //devolvemos el resultado del metodo
            return aResultado;

        }


        #endregion


        #region METODOS ESPECIFICOS

        /// <summary>
        /// Devuelve una referencia a la conexion misma a la base de datos
        /// </summary>
        public OracleConnection Conexion
        {
            get
            {
                //si la conexion no esta abierta actualmente, la intentamos abrir
                if (__oConexion.State != ConnectionState.Open) aConectar();

                //devolvemos la conexion
                return __oConexion;
            }
        }

        /// <summary>
        /// Ejecuta la consulta a partir de una instancia de Comando
        /// </summary>
        /// <param name="cmdComandoParam">Instancia de Comando a ejecutar</param>
        /// <returns>Lista de resultados List[int, DataTable, DataAdapter]</returns>
        public object[] aEjecutar_consulta(OracleCommand cmdComandoParam)
        {
            string NOMBRE_METODO = NOMBRE_CLASE + ".lEjecutar_consulta()";

            object[] aResultado = new object[] { 0, NOMBRE_METODO + " No Ejecutado." };

            try
            {
                //configuramos el comando
                cmdComandoParam.Connection = this.Conexion;
                cmdComandoParam.CommandTimeout = nTiempoComandos;

                //instanciamos los adaptadores
                OracleDataAdapter daAdaptador = new OracleDataAdapter(cmdComandoParam);
                DataTable dtTabla = new DataTable();

                _Mostrar_SQL(cmdComandoParam.CommandText);

                //recuperamos los datos para la tabla
                daAdaptador.Fill(dtTabla);

                //establecemos el resultado del metodo
                aResultado = new object[] { 1, dtTabla, daAdaptador };

            }
            catch (Exception ex)
            {
                //en caso de error
                aResultado = new object[] { -1, NOMBRE_METODO + ": " + ex.Message + (char)13 + "Sentencia -> " + cmdComandoParam };
            }

            //devolvemos el resultado del metodo
            return aResultado;

        }

        #endregion

    }
}
