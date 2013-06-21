using System;
using System.Collections.Generic;

using System.Text;
using System.Data;
using CdgNetPersistenciaV3.ClasesBases;
using System.Data.SqlClient;

namespace CdgNetPersistenciaV3
{

    /// <summary>
    /// Utileria para interaccion con Base de datos MS SQL Server
    /// </summary>
    public class SQLServerUtiles : ConectorBase
    {

        #region CAMPOS(Fields) DE LA CLASE

        /// <summary>
        /// Nombre de la clase
        /// </summary>
        public const string NOMBRE_CLASE = "SQLServerUtiles";

        /// <summary>
        /// Modo de Autenticacion por Usuario del SO
        /// </summary>
        public const string AUTENTICACION_WINDOWS = "Autenticacion de Windows";

        /// <summary>
        /// Modo de Autenticacion por Login de la BDD
        /// </summary>
        public const string AUTENTICACION_SQLSERVER = "Autenticacion de Sql Server";

        /// <summary>
        /// Arreglo de los Modos de Conexion Disponibles
        /// </summary>
        public static string[] LISTA_MODOS_AUTENTICACIONES = new String[2] { AUTENTICACION_WINDOWS, AUTENTICACION_SQLSERVER };

        /// <summary>
        /// Arreglo de Nombres Comunes de Instancias Locales
        /// </summary>
        public static string[] LISTA_INSTANCIAS_LOCALES = new String[2] { "SQLEXPRESS", "MSSQLSERVER" };

        /// <summary>
        /// Cadena de Conexion para Usuario de SO
        /// </summary>
        private const string CADENA_CONEXION_WINDOWS = "Server={0}; Database={1}; Integrated Security= yes";

        /// <summary>
        /// Cadena de Conexion para Login de SQL Server
        /// </summary>
        private const string CADENA_CONEXION_SQLSERVER = "Data Source={0}; Initial Catalog={1};User Id={2};Password={3}";

        /// <summary>
        /// Caracter de marca de parametros de comando T-SQL
        /// </summary>
        public new static char MARCADOR_PARAMETRO = '@';

        private SqlConnectionStringBuilder __oCadenaConexion;
        //public new SqlConnection oConexion;
        private SqlTransaction __oTransaccionSQL;


        /// <summary>
        /// Devuelve o establece el Tipo de Conexion 
        /// </summary>
        public string cTipoConexion { get; set; }


        public enum PARAM_ESPECIALES { TBL_BASE, DATASET, ADM_BASE };

        #endregion


        #region CONSTRUCTORES DE LA CLASE


        /// <summary>
        /// Constructor de la clase
        /// </summary>
        /// <param name="cCadenaConexionParam">Cadena de conexion personalizada</param>
        public SQLServerUtiles(string cCadenaConexionParam)
            : base(cCadenaConexionParam)
        {
            cTipoConexion = AUTENTICACION_SQLSERVER;

            //tomamos la cadena de conexion
            __oCadenaConexion = new SqlConnectionStringBuilder(cCadenaConexionParam);

        }

        /// <summary>
        /// Constructor de la clase
        /// </summary>
        /// <param name="cServidorParam">Nombre del servidor de base de datos</param>
        /// <param name="cCatalogoParam">Catalogo de base de datos</param>
        /// <param name="cUsuarioParam">ID de Usuario</param>
        /// <param name="cContrasenaParam">Contrasena</param>
        /// <param name="nTiempoComandosParam">Tiempo de espera de respuesta a comandos</param>
        public SQLServerUtiles(string cServidorParam, string cCatalogoParam
                                , string cUsuarioParam, string cContrasenaParam
                                , int nTiempoComandosParam)
            : base(cUsuarioParam, cContrasenaParam, cServidorParam, cCatalogoParam, 1433, string.Empty)
        {
            cTipoConexion = AUTENTICACION_SQLSERVER;

            //formateamos la cadena de conexion
            __oCadenaConexion = new SqlConnectionStringBuilder(string.Format(CADENA_CONEXION_SQLSERVER
                                                                            , _cServidor
                                                                            , _cCatalogo
                                                                            , _cUsuario
                                                                            , _cContrasena)
                                                                );
            nTiempoComandos = nTiempoComandos;

        }

        /// <summary>
        /// Constructor de la clase
        /// </summary>
        /// <param name="cServidorParam">Nombre del servidor de base de datos</param>
        /// <param name="cCatalogoParam">Catalogo de base de datos</param>
        /// <param name="nTiempoComandosParam">Tiempo de espera de respuesta a comandos</param>
        public SQLServerUtiles(string cServidorParam, string cCatalogoParam, int nTiempoComandosParam)
            : base(string.Empty, string.Empty, cServidorParam, cCatalogoParam, 1433, string.Empty)
        {
            cTipoConexion = AUTENTICACION_WINDOWS;

            //formateamos la cadena de conexion
            __oCadenaConexion = new SqlConnectionStringBuilder(string.Format(CADENA_CONEXION_WINDOWS
                                                                , _cServidor
                                                                , _cCatalogo)
                                                            );

            nTiempoComandos = nTiempoComandos;

        }


        #endregion


        #region Miembros de ConectorBase

        /// <summary>
        /// Abre la conexion a la base de datos
        /// </summary>
        /// <returns>Lista de Resultados</returns>
        public override List<object> lConectar()
        {
            string NOMBRE_METODO = NOMBRE_CLASE + ".lConectar()";
            List<object> lResultado = new List<object>() { 0, NOMBRE_METODO + " No Ejecutado." };

            try
            {
                //intentamos abrir la conexion a la base de datos
                oConexion = new SqlConnection(__oCadenaConexion.ConnectionString);

                oConexion.Open();

                lResultado = new List<object>() { 1, "Ok" };

            }
            catch (Exception ex)
            {
                //en caso de error
                lResultado = new List<object>() { -1, NOMBRE_METODO + ": " + ex.Message };
            }

            //devolvemos el resultado del metodo
            return lResultado;

        }

        /// <summary>
        /// Cierra la conexion activa
        /// </summary>
        /// <param name="sender">Objeto que efectua la llamada al metodo</param>
        /// <returns>Lista de Resultados</returns>
        public override List<object> lDesconectar(object sender)
        {
            string NOMBRE_METODO = NOMBRE_CLASE + ".lDesconectar()";
            List<object> lResultado = new List<object>() { 0, NOMBRE_METODO + " No Ejecutado." };

            try
            {
                //si la conexion esta abierta, la intentamos cerrar la conexion a la base de datos
                if (oConexion.State == System.Data.ConnectionState.Open) this.Conexion.Close();
                GC.Collect();
                _bConectado = false;

                lResultado = new List<object>() { 1, "Ok" };
            }
            catch (Exception ex)
            {
                //en caso de error
                lResultado = new List<object>() { -1, NOMBRE_METODO + ": " + ex.Message };
            }

            //devolvemos el resultado del metodo
            return lResultado;
        }

        /// <summary>
        /// Ejecuta la sentencia sql
        /// </summary>
        /// <param name="cSentenciaSQL">Sentencia SQL a ejecutar</param>
        /// <param name="dicParametros">Diccionario de parametros a combinar con la sentencia</param>
        /// <returns>Lista de Resultado[int, object]</returns>
        public override List<object> lEjecutar_sentencia(string cSentenciaSQL, Dictionary<string, object> dicParametros)
        {
            string NOMBRE_METODO = NOMBRE_CLASE + ".lEjecutar_sentencia()";

            List<object> lResultado = new List<object>() { 0, NOMBRE_METODO + " No Ejecutado." };

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
                SqlCommand oComandoSQL;

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
                    oComandoSQL = new SqlCommand(cSentenciaSQL);
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
                lResultado = new List<object>() { 1, "Ok" };

            }
            catch (Exception ex)
            {
                //en caso de error
                lResultado = new List<object>() { -1, NOMBRE_METODO + ": " + ex.Message + (char)13 + "Sentencia -> " + cSentenciaSQL };
            }

            //devolvemos el resultado del metodo
            return lResultado;

        }

        /// <summary>
        /// Ejecuta la consulta sql y devuelve un DataTable como segundo elemento
        /// </summary>
        /// <param name="cConsultaSQL">Consulta SQL a ejecutar</param>
        /// <param name="dicParametros">Diccionario de parametros a combinar con la consulta</param>
        /// <returns>Lista de Resultado[int, object]</returns>
        public override List<object> lEjecutar_consulta(string cConsultaSQL, Dictionary<string, object> dicParametros)
        {
            string NOMBRE_METODO = NOMBRE_CLASE + ".lEjecutar_consulta()";
            List<object> lResultado = new List<object>() { 0, NOMBRE_METODO + " No Ejecutado." };

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
                SqlCommand oComandoSQL;

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
                    oComandoSQL = new SqlCommand(cConsultaSQL);
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

                SqlDataAdapter daAdaptador = new SqlDataAdapter(oComandoSQL);
                DataTable dtTabla = new DataTable();

                _Mostrar_SQL(oComandoSQL.CommandText);

                //el resultado a la tabla
                daAdaptador.Fill(dtTabla);

                //si hay filas devueltas
                if (dtTabla.Rows.Count > 0) lResultado = new List<object>() { 1, dtTabla, daAdaptador };
                else lResultado = new List<object>() { 0, ConectorBase.ERROR_NO_HAY_FILAS, oComandoSQL.CommandText };

            }
            catch (Exception ex)
            {
                //en caso de error
                lResultado = new List<object>() { -1, NOMBRE_METODO + ": " + ex.Message + (char)13 + "Sentencia -> " + cConsultaSQL };
            }

            //devolvemos el resultado del metodo
            return lResultado;

        }

        /// <summary>
        /// Ejecuta la consulta sql y devuelve un unico objeto valor
        /// </summary>
        /// <param name="cConsultaSQL">Consulta SQL a ejecutar</param>
        /// <param name="cConsultaSQL">Consulta a ejecutar</param>
        /// <param name="dicParametros">Diccionario de parametros a combinar con la consulta</param>
        /// <returns>Lista de Resultado[int, object]</returns>
        public override List<object> lEjecutar_escalar(string cConsultaSQL, Dictionary<string, object> dicParametros)
        {
            string NOMBRE_METODO = NOMBRE_CLASE + ".lEjecutar_escalar()";
            List<object> lResultado = new List<object>() { 0, NOMBRE_METODO + " No Ejecutado." };

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
                SqlCommand oComandoSQL;

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
                    oComandoSQL = new SqlCommand(cConsultaSQL);
                    oComandoSQL.Connection = this.Conexion;
                }

                //preparamos la sentencia a ejecutar
                oComandoSQL.Prepare();

                //configuramos el comando
                oComandoSQL.CommandTimeout = nTiempoComandos;

                _Mostrar_SQL(oComandoSQL.CommandText);

                //recorremos el diccionario de parametros
                foreach (string cNombre in dicParametros.Keys)
                {
                    //si el valor NO es nulo
                    if (dicParametros[cNombre] != null)
                        //lo agregamos al comando los parametros y sus valores
                        oComandoSQL.Parameters.AddWithValue(cNombre, dicParametros[cNombre]);
                }

                //ejecutamos la consulta
                object oValor = oComandoSQL.ExecuteScalar();

                //establecemos el resultado del metodo
                lResultado = new List<object>() { 1, oValor };

            }
            catch (Exception ex)
            {
                //en caso de error
                lResultado = new List<object>() { -1, NOMBRE_METODO + ": " + ex.Message + (char)13 + "Sentencia -> " + cConsultaSQL };
            }

            //devolvemos el resultado del metodo
            return lResultado;

        }

        /// <summary>
        /// Ejecuta el procedimiento almacenado y devuelve una lista de resultado
        /// </summary>
        /// <param name="cProcedimiento">Procedimiento Almacenado a ejecutar</param>
        /// <param name="dicParametros">Diccionario de parametros a combinar con la consulta</param>
        /// <returns>Lista de Resultados</returns>
        public override List<object> lEjecutar_procedimiento(string cProcedimiento, Dictionary<string, object> dicParametros)
        {
            string NOMBRE_METODO = NOMBRE_CLASE + ".lEjecutar_sp_parametros()";
            List<object> lResultado = new List<object>() { 0, NOMBRE_METODO + " No Ejecutado." };

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
                SqlCommand oComandoSQL;

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
                    oComandoSQL = new SqlCommand(cProcedimiento);
                    oComandoSQL.Connection = this.Conexion;
                }

                //preparamos la sentencia a ejecutar
                oComandoSQL.Prepare();

                //configuramos el comando
                oComandoSQL.CommandTimeout = nTiempoComandos;

                _Mostrar_SQL(oComandoSQL.CommandText);

                //recorremos el diccionario de parametros
                foreach (string cNombre in dicParametros.Keys)
                {
                    //si el valor NO es nulo
                    if (dicParametros[cNombre] != null)
                        //lo agregamos al comando los parametros y sus valores
                        oComandoSQL.Parameters.AddWithValue(cNombre, dicParametros[cNombre]);
                }

                //ejecutamos la consulta
                object oValor = oComandoSQL.ExecuteNonQuery();

                //establecemos el resultado del metodo
                lResultado = new List<object>() { 1, "Ok" };

            }
            catch (Exception ex)
            {
                //en caso de error
                lResultado = new List<object>() { -1, NOMBRE_METODO + ": " + ex.Message };
            }

            //devolvemos el resultado del metodo
            return lResultado;
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
        /// <returns>Lista de Resultados</returns>
        public override List<object> lIniciar_transaccion()
        {
            string NOMBRE_METODO = NOMBRE_CLASE + ".lIniciar_transaccion()";
            List<object> lResultado = new List<object>() { 0, NOMBRE_METODO + " No Ejecutado." };

            try
            {
                //si hay una transaccion activa
                if (__oTransaccionSQL != null)
                    //generamos un error
                    throw new Exception("Hay una transaccion pendiente! \n Confirmela o reviertala antes de iniciar otra...");

                //iniciamos una transaccion
                __oTransaccionSQL = (oConexion as SqlConnection).BeginTransaction();

                lResultado = new List<object>() { 1, "Ok" };
            }
            catch (Exception ex)
            {
                //en caso de error
                lResultado = new List<object>() { -1, NOMBRE_METODO + ": " + ex.Message };
            }

            //devolvemos el resultado del metodo
            return lResultado;
        }

        /// <summary>
        /// Confirma una transaccion activa
        /// </summary>
        /// <returns>Lista de Resultados</returns>
        public override List<object> lConfirmar_transaccion()
        {
            string NOMBRE_METODO = NOMBRE_CLASE + ".lConfirmar_transaccion()";
            List<object> lResultado = new List<object>() { 0, NOMBRE_METODO + " No Ejecutado." };

            try
            {
                //confirmamos la transaccion
                __oTransaccionSQL.Commit();

                //la liberamos
                __oTransaccionSQL.Dispose();
                __oTransaccionSQL = null;

                lResultado = new List<object>() { 1, "Ok" };
            }
            catch (Exception ex)
            {
                //en caso de error
                lResultado = new List<object>() { -1, NOMBRE_METODO + ": " + ex.Message };
            }

            //devolvemos el resultado del metodo
            return lResultado;
        }

        /// <summary>
        /// Revierte una transaccion activa
        /// </summary>
        /// <returns>Lista de Resultados</returns>
        public override List<object> lRevertir_transaccion()
        {
            string NOMBRE_METODO = NOMBRE_CLASE + ".lConectar()";
            List<object> lResultado = new List<object>() { 0, NOMBRE_METODO + " No Ejecutado." };

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
                lResultado = new List<object>() { 1, "Ok" };
            }
            catch (Exception ex)
            {
                //en caso de error
                lResultado = new List<object>() { -1, NOMBRE_METODO + ": " + ex.Message };
            }

            //devolvemos el resultado del metodo
            return lResultado;
        }



        #endregion


        #region METODOS ESPECIFICOS

        /// <summary>
        /// Ejecuta una consulta y devuelve un datatable y su adaptador de datos
        /// </summary>
        /// <param name="cConsultaSQL">Consulta SQL a ejecutar</param>
        /// <param name="dicParametros">Diccionario de Parametros [string, object]</param>
        /// <returns>Lista de resultados [int, object, object]</returns>
        public List<object> lEjecutar_consulta_especial(string cConsultaSQL, Dictionary<string, object> dicParametros)
        {
            string NOMBRE_METODO = NOMBRE_CLASE + ".lEjecutar_consulta()";
            List<object> lResultado = new List<object>() { 0, NOMBRE_METODO + " No Ejecutado." };

            try
            {
                //creamos una instancia de comando
                SqlCommand oComandoSQL = new SqlCommand(cConsultaSQL);

                //configuramos el comando
                oComandoSQL.CommandTimeout = nTiempoComandos;

                //si hay una transaccion activa
                if (__oTransaccionSQL != null)
                {
                    //asignamos el comando como parte de la misma
                    oComandoSQL.Connection = __oTransaccionSQL.Connection;
                    oComandoSQL.Transaction = __oTransaccionSQL;
                }
                else
                {
                    //sino, la conexion actual de la clase
                    oComandoSQL.Connection = this.Conexion;
                }

                SqlDataAdapter daAdaptador = new SqlDataAdapter(oComandoSQL);
                DataTable dtTabla = new DataTable();

                _Mostrar_SQL(oComandoSQL.CommandText);

                //si se paso la instancia de la tabla y su dataset
                if (dicParametros.ContainsKey(PARAM_ESPECIALES.ADM_BASE.ToString())
                        && dicParametros.ContainsKey(PARAM_ESPECIALES.DATASET.ToString()))
                {
                    //tomamos la instancia TBL parametro
                    ADMbase oADMbase = (ADMbase)dicParametros[PARAM_ESPECIALES.ADM_BASE.ToString()];

                    //nombre del mapeo
                    //string cNombreMap = string.Format("map{0}", oTBLbase.Get_nombre_tabla());
                    string cNombreMap = oADMbase.NombreTabla;

                    //creamos una instancia de mapeo de tablas
                    System.Data.Common.DataTableMapping tmMapeo = new System.Data.Common.DataTableMapping(
                                                                    oADMbase.NombreTabla
                                                                    , cNombreMap
                                                                    );

                    //lo agregamos al adaptador
                    daAdaptador.TableMappings.Add(tmMapeo);

                    //recuperamos los datos
                    daAdaptador.Fill((DataSet)dicParametros[PARAM_ESPECIALES.DATASET.ToString()], cNombreMap);

                }
                else
                {
                    //sino, normalmente
                    daAdaptador.Fill(dtTabla);
                }

                //si hay filas devueltas
                if (dtTabla.Rows.Count > 0) lResultado = new List<object>() { 1, dtTabla, daAdaptador };
                else lResultado = new List<object>() { 0, ConectorBase.ERROR_NO_HAY_FILAS };

            }
            catch (Exception ex)
            {
                //en caso de error
                lResultado = new List<object>() { -1, NOMBRE_METODO + ": " + ex.Message };
            }

            //devolvemos el resultado del metodo
            return lResultado;

        }

        /// <summary>
        /// Ejecuta una sentencia sql pero a partir de una instancia de Comando
        /// </summary>
        /// <param name="cmdComandoSQL">Instancia del Comando a ejecutar</param>
        /// <returns>Lista de resultados List[int, object]</returns>
        public List<object> lEjecutar_sentencia(SqlCommand cmdComandoSQL)
        {
            string NOMBRE_METODO = NOMBRE_CLASE + ".lEjecutar_sentencia()";
            List<object> lResultado = new List<object>() { 0, NOMBRE_METODO + " No Ejecutado." };

            try
            {
                //configuramos el comando
                cmdComandoSQL.Connection = this.Conexion;
                cmdComandoSQL.CommandTimeout = nTiempoComandos;

                //si hay una transaccion activa, asignamos el comando como parte de la misma
                //if (__oTransaccionSQL != null)
                cmdComandoSQL.Transaction = __oTransaccionSQL;

                _Mostrar_SQL(cmdComandoSQL.CommandText);

                cmdComandoSQL.ExecuteNonQuery();

                lResultado = new List<object>() { 1, "Ok" };
            }
            catch (SqlException ex)
            {
                //en caso de error
                lResultado = new List<object>() { -1, NOMBRE_METODO + ": " + ex.Message, ex };
            }
            catch (Exception ex)
            {
                //en caso de error
                lResultado = new List<object>() { -1, NOMBRE_METODO + ": " + ex.Message, ex };
            }

            //devolvemos el resultado del metodo
            return lResultado;
        }

        /// <summary>
        /// Ejecuta una insercion masiva desde el datatable parametro a la tabla especificada
        /// los campos y orden de los mismos deben de coincidir
        /// </summary>
        /// <param name="dtDataTableParam">DataTable origen de datos</param>
        /// <param name="cTablaDestinoParam">Nombre tabla destino</param>
        /// <returns>Lista de resultados List[int, object]</returns>
        public List<object> lEjecutar_bulkInsert(DataTable dtDataTableParam
                                                    , string cTablaDestinoParam)
        {
            string NOMBRE_METODO = NOMBRE_CLASE + ".lEjecutar_bulkInsert()";
            List<object> lResultado = new List<object>() { 0, NOMBRE_METODO + " No Ejecutado." };

            try
            {
                //configuramos el comando
                SqlBulkCopy bulkCopy = new SqlBulkCopy((oConexion as SqlConnection), SqlBulkCopyOptions.CheckConstraints, __oTransaccionSQL);

                bulkCopy.BulkCopyTimeout = nTiempoComandos;
                bulkCopy.DestinationTableName = cTablaDestinoParam;
                bulkCopy.BatchSize = 100;
                bulkCopy.NotifyAfter = 100;

                //ejecutamos el comando
                bulkCopy.WriteToServer(dtDataTableParam);

                //resultado del metodo
                lResultado = new List<object>() { 1, "Ok" };

            }
            catch (Exception ex)
            {
                //en caso de error
                lResultado = new List<object>() { -1, NOMBRE_METODO + ": " + ex.Message, ex };
            }

            //devolvemos el resultado del metodo
            return lResultado;

        }

        /// <summary>
        /// Devuelve una referencia a la conexion misma a la base de datos
        /// </summary>
        public SqlConnection Conexion
        {
            get
            {
                //si la conexion no esta abierta actualmente, la intentamos abrir
                if (oConexion.State != ConnectionState.Open) lConectar();

                //devolvemos la conexion
                return oConexion as SqlConnection;
            }
        }



        #endregion



    }
}
