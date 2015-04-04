using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CdgNetPersistenciaV3_5.ClasesBases;
using MySql.Data.MySqlClient;
using System.Data;

namespace CdgNetPersistenciaV3_5
{
    /// <summary>
    /// Utileria para interaccion con Base de datos MySQL / MariaDB
    /// </summary>
    public class MySqlUtiles : ConectorBase
    {
        #region CAMPOS(Fields) DE LA CLASE

        /// <summary>
        /// Nombre de la clase
        /// </summary>
        public const string NOMBRE_CLASE = "MySqlUtiles";

        /// <summary>
        /// Cadena de Conexion de SO
        /// </summary>
        private const string CADENA_CONEXION_MYSQL = "Server={0};Port={1};Database={2};Uid={3};Pwd={4};Connection Timeout={5};Pooling=True;";

        /// <summary>
        /// Caracter de marca de parametros de comando T-SQL
        /// </summary>
        public new static char MARCADOR_PARAMETRO = '?';

        private MySqlConnectionStringBuilder __oCadenaConexion;
        private MySqlTransaction __oTransaccionSQL;

        public enum PARAM_ESPECIALES { TBL_BASE, DATASET, ADM_BASE };

        #endregion


        #region CONSTRUCTORES DE LA CLASE


        /// <summary>
        /// Constructor de la clase
        /// </summary>
        /// <param name="cServidorParam">Nombre del servidor de base de datos</param>
        /// <param name="cEsquemaParam">Nombre de base de datos</param>
        /// <param name="cUsuarioParam">ID de Usuario</param>
        /// <param name="cContrasenaParam">Contrasena</param>
        /// <param name="nTiempoComandosParam">Tiempo de espera de respuesta a comandos</param>
        /// <param name="nPuertoParam">Numero de puerto de escucha del servicio</param>
        public MySqlUtiles(string cServidorParam, string cEsquemaParam
                                , string cUsuarioParam, string cContrasenaParam
                                , int nTiempoComandosParam, int nPuertoParam)
            : base(cUsuarioParam, cContrasenaParam, cServidorParam, cEsquemaParam, nPuertoParam, string.Empty)
        {
            //formateamos la cadena de conexion
            __oCadenaConexion = new MySqlConnectionStringBuilder(string.Format(CADENA_CONEXION_MYSQL
                                                                            , _cServidor
                                                                            , _nPuerto
                                                                            , _cCatalogo
                                                                            , _cUsuario
                                                                            , _cContrasena
                                                                            , nTiempoComandosParam
                                                                            )
                                                                );
            nTiempoComandos = nTiempoComandos;

        }


        #endregion


        #region Miembros de ConectorBase

        /// <summary>
        /// Abre la conexion a la base de datos
        /// </summary>
        /// <returns>Arreglo de Resultados [int, object]</returns>
        public override object[] aConectar()
        {
            string NOMBRE_METODO = NOMBRE_CLASE + ".lConectar()";
            object[] aResultado = new object[] { 0, NOMBRE_METODO + " No Ejecutado." };

            try
            {
                //intentamos abrir la conexion a la base de datos
                oConexion = new MySqlConnection(__oCadenaConexion.ConnectionString);

                oConexion.Open();

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
        /// Cierra la conexion activa
        /// </summary>
        /// <param name="sender">Objeto que efectua la llamada al metodo</param>
        /// <returns>Arreglo de Resultados [int, object]</returns>
        public override object[] aDesconectar(object sender)
        {
            string NOMBRE_METODO = NOMBRE_CLASE + ".lDesconectar()";
            object[] aResultado = new object[] { 0, NOMBRE_METODO + " No Ejecutado." };

            try
            {
                //si la conexion esta abierta, la intentamos cerrar la conexion a la base de datos
                if (oConexion.State == System.Data.ConnectionState.Open) this.Conexion.Close();
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
        /// Ejecuta la sentencia sql
        /// </summary>
        /// <param name="cSentenciaSQL">Sentencia SQL a ejecutar</param>
        /// <param name="dicParametros">Diccionario de parametros a combinar con la sentencia</param>
        /// <returns>Arreglo de Resultados[int, object]</returns>
        public override object[] aEjecutar_sentencia(string cSentenciaSQL, Dictionary<string, object> dicParametros)
        {
            string NOMBRE_METODO = NOMBRE_CLASE + ".lEjecutar_sentencia()";

            object[] aResultado = new object[] { 0, NOMBRE_METODO + " No Ejecutado." };

            try
            {
                //si se recibe el diccionario de parametros 
                if (dicParametros != null)
                {
                    //lo recorremos
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
                }

                //creamos una instancia de comando
                MySqlCommand oComandoSQL;

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
                    oComandoSQL = new MySqlCommand(cSentenciaSQL);
                    oComandoSQL.Connection = this.Conexion;
                }

                //preparamos la sentencia a ejecutar
                oComandoSQL.Prepare();

                //configuramos el comando
                oComandoSQL.CommandTimeout = nTiempoComandos;

                //si se recibe el diccionario de parametros 
                if (dicParametros != null)
                {
                    //lo recorremos
                    foreach (string cNombre in dicParametros.Keys)
                    {
                        //si el valor NO es nulo
                        if (dicParametros[cNombre] != null)
                            //lo agregamos al comando los parametros y sus valores
                            oComandoSQL.Parameters.AddWithValue(cNombre, dicParametros[cNombre]);
                    }
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
        /// Ejecuta la consulta sql y devuelve un DataTable como segundo elemento
        /// </summary>
        /// <param name="cConsultaSQL">Consulta SQL a ejecutar</param>
        /// <param name="dicParametros">Diccionario de parametros a combinar con la consulta</param>
        /// <returns>Arreglo de Resultados[int, object]</returns>
        public override object[] aEjecutar_consulta(string cConsultaSQL, Dictionary<string, object> dicParametros)
        {
            string NOMBRE_METODO = NOMBRE_CLASE + ".lEjecutar_consulta()";
            object[] aResultado = new object[] { 0, NOMBRE_METODO + " No Ejecutado." };

            try
            {
                //si se recibe el diccionario de parametros 
                if (dicParametros != null)
                {
                    //lo recorremos
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
                }

                //creamos una instancia de comando
                MySqlCommand oComandoSQL;

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
                    oComandoSQL = new MySqlCommand(cConsultaSQL);
                    oComandoSQL.Connection = this.Conexion;
                }

                //preparamos la sentencia a ejecutar
                oComandoSQL.Prepare();

                //configuramos el comando
                oComandoSQL.CommandTimeout = nTiempoComandos;

                //si se recibe el diccionario de parametros 
                if (dicParametros != null)
                {
                    //lo recorremos
                    foreach (string cNombre in dicParametros.Keys)
                    {
                        //si el valor NO es nulo
                        if (dicParametros[cNombre] != null)
                            //lo agregamos al comando los parametros y sus valores
                            oComandoSQL.Parameters.AddWithValue(cNombre, dicParametros[cNombre]);
                    }
                }

                MySqlDataAdapter daAdaptador = new MySqlDataAdapter(oComandoSQL);
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
        /// Ejecuta la consulta sql y devuelve un unico objeto valor
        /// </summary>
        /// <param name="cConsultaSQL">Consulta SQL a ejecutar</param>
        /// <param name="cConsultaSQL">Consulta a ejecutar</param>
        /// <param name="dicParametros">Diccionario de parametros a combinar con la consulta</param>
        /// <returns>Arreglo de Resultados[int, object]</returns>
        public override object[] aEjecutar_escalar(string cConsultaSQL, Dictionary<string, object> dicParametros)
        {
            string NOMBRE_METODO = NOMBRE_CLASE + ".lEjecutar_escalar()";
            object[] aResultado = new object[] { 0, NOMBRE_METODO + " No Ejecutado." };

            try
            {
                //si se recibe el diccionario de parametros 
                if (dicParametros != null)
                {
                    //lo recorremos
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
                }

                //creamos una instancia de comando
                MySqlCommand oComandoSQL;

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
                    oComandoSQL = new MySqlCommand(cConsultaSQL);
                    oComandoSQL.Connection = this.Conexion;
                }

                //preparamos la sentencia a ejecutar
                oComandoSQL.Prepare();

                //configuramos el comando
                oComandoSQL.CommandTimeout = nTiempoComandos;

                _Mostrar_SQL(oComandoSQL.CommandText);

                //si se recibe el diccionario de parametros 
                if (dicParametros != null)
                {
                    //lo recorremos
                    foreach (string cNombre in dicParametros.Keys)
                    {
                        //si el valor NO es nulo
                        if (dicParametros[cNombre] != null)
                            //lo agregamos al comando los parametros y sus valores
                            oComandoSQL.Parameters.AddWithValue(cNombre, dicParametros[cNombre]);
                    }
                }

                //ejecutamos la consulta
                object oValor = oComandoSQL.ExecuteScalar();

                //establecemos el resultado del metodo
                aResultado = new object[] { 1, oValor };

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
        /// Ejecuta el procedimiento almacenado y devuelve una lista de resultado
        /// </summary>
        /// <param name="cProcedimiento">Procedimiento Almacenado a ejecutar</param>
        /// <param name="dicParametros">Diccionario de parametros a combinar con la consulta</param>
        /// <returns>Arreglo de Resultados [int, object]</returns>
        public override object[] aEjecutar_procedimiento(string cProcedimiento, Dictionary<string, object> dicParametros)
        {
            string NOMBRE_METODO = NOMBRE_CLASE + ".lEjecutar_sp_parametros()";
            object[] aResultado = new object[] { 0, NOMBRE_METODO + " No Ejecutado." };

            try
            {
                //si se recibe el diccionario de parametros 
                if (dicParametros != null)
                {
                    //lo recorremos
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
                }

                //creamos una instancia de comando
                MySqlCommand oComandoSQL;

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
                    oComandoSQL = new MySqlCommand(cProcedimiento);
                    oComandoSQL.Connection = this.Conexion;
                }

                //preparamos la sentencia a ejecutar
                oComandoSQL.Prepare();

                //configuramos el comando
                oComandoSQL.CommandTimeout = nTiempoComandos;

                _Mostrar_SQL(oComandoSQL.CommandText);

                //si se recibe el diccionario de parametros 
                if (dicParametros != null)
                {
                    //lo recorremos
                    foreach (string cNombre in dicParametros.Keys)
                    {
                        //si el valor NO es nulo
                        if (dicParametros[cNombre] != null)
                            //lo agregamos al comando los parametros y sus valores
                            oComandoSQL.Parameters.AddWithValue(cNombre, dicParametros[cNombre]);
                    }
                }

                //ejecutamos la consulta
                object oValor = oComandoSQL.ExecuteNonQuery();

                //establecemos el resultado del metodo
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
        /// Establece el metodo de Autoconfirmacion de transacciones
        /// </summary>
        /// <param name="bParam">Valor a asignar</param>
        public override void Set_autocommit(bool bParam)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Inicia una transaccion
        /// </summary>
        /// <returns>Arreglo de Resultados [int, object]</returns>
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
                __oTransaccionSQL = (oConexion as MySqlConnection).BeginTransaction();

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
        /// Confirma una transaccion activa
        /// </summary>
        /// <returns>Arreglo de Resultados [int, object]</returns>
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
        /// Revierte una transaccion activa
        /// </summary>
        /// <returns>Arreglo de Resultados [int, object]</returns>
        public override object[] aRevertir_transaccion()
        {
            string NOMBRE_METODO = NOMBRE_CLASE + ".lConectar()";
            object[] aResultado = new object[] { 0, NOMBRE_METODO + " No Ejecutado." };

            try
            {
                //si existe una transaccion activa
                if (__oTransaccionSQL != null)
                {
                    //revertimos la transaccion
                    __oTransaccionSQL.Rollback();

                    //la liberamos
                    __oTransaccionSQL.Dispose();
                    __oTransaccionSQL = null;
                }

                //establecemos el resultado del metodo
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


        #region ESPECIFICOS

        /// <summary>
        /// Devuelve una referencia a la conexion misma a la base de datos
        /// </summary>
        public MySqlConnection Conexion
        {
            get
            {
                //si la conexion no esta abierta actualmente, la intentamos abrir
                if (oConexion.State != ConnectionState.Open) aConectar();

                //devolvemos la conexion
                return oConexion as MySqlConnection;
            }
        }



        #endregion


    }
}
