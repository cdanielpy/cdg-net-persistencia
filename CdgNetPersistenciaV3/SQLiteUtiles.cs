using System;
using System.Collections.Generic;

using System.Text;
using System.Data;
using System.Data.SQLite;
using CdgNetPersistenciaV3.ClasesBases;
using System.IO;

namespace CdgNetPersistenciaV3
{
    /// <summary>
    /// Utileria para interaccion con Base de datos SQLite
    /// </summary>
    public class SQLiteUtiles : ConectorBase
    {

        #region ATRIBUTOS DE LA CLASE

        /// <summary>
        /// Nombre de la clase
        /// </summary>
        public const string NOMBRE_CLASE = "SQLiteUtiles";

        /// <summary>
        /// Caracter de marca de parametros de comandos SQL
        /// </summary>
        public new static char MARCADOR_PARAMETRO = ':';

        private SQLiteConnectionStringBuilder __oCadenaConexion;
        private SQLiteConnection __oConexion;
        private SQLiteTransaction __oTransaccionSQL;

        public const string CADENA_CONEXION_STD = "Data Source={0}; Version=3; Journal Mode=Off; Pooling=True;Max Pool Size=10;";

        #endregion


        #region CONSTRUCTORES DE LA CLASE

        /// <summary>
        /// Cadena de Conexion valida para SQLite
        /// </summary>
        /// <param name="cConexionSQLite">Cadena de Conexion o el Path al archivo SQLite</param>
        public SQLiteUtiles(string cConexionSQLite)
            : base(string.Empty, string.Empty, string.Empty, string.Empty, 0, string.Empty)
        {
            //si lo que recibimos es un directorio, lo formateamos a al cadena 
            if (File.Exists(cConexionSQLite)) cConexionSQLite = string.Format(CADENA_CONEXION_STD, cConexionSQLite);
            __oCadenaConexion = new SQLiteConnectionStringBuilder(cConexionSQLite);
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
                __oConexion = new SQLiteConnection(__oCadenaConexion.ConnectionString);

                __oConexion.Open();
                _bConectado = true;

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
                if (__oConexion.State == System.Data.ConnectionState.Open) Conexion.Close();
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
        /// Ejecuta la sentencia parametro
        /// </summary>
        /// <param name="cSentenciaSQL">Sentencia a Ejecutar</param>
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
                SQLiteCommand oComandoSQL;

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
                    oComandoSQL = new SQLiteCommand(cSentenciaSQL);
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

                //ejecutamos el comando
                oComandoSQL.ExecuteNonQuery();

                //establecemos el resultado
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
        /// Ejecuta la consulta parametro y devuelve la lista de resultados
        /// </summary>
        /// <param name="cConsultaSQL">Consulta a ejecutar</param>
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
                SQLiteCommand oComandoSQL;

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
                    oComandoSQL = new SQLiteCommand(cConsultaSQL);
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

                //un adaptador de datos
                SQLiteDataAdapter daAdaptador = new SQLiteDataAdapter(oComandoSQL);

                //una tabla para retornar los resultados
                DataTable dtTabla = new DataTable();

                _Mostrar_SQL(oComandoSQL.CommandText);

                //cargamos la tabla destino
                daAdaptador.Fill(dtTabla);

                //si hay filas devueltas
                if (dtTabla.Rows.Count > 0) lResultado = new List<object>() { 1, dtTabla, daAdaptador };
                else lResultado = new List<object>() { 0, ConectorBase.ERROR_NO_HAY_FILAS, oComandoSQL.CommandText };

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
        /// Ejecuta la consulta parametro y devuelve el valor del primer campo
        /// de la primera fila recuperada
        /// </summary>
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
                SQLiteCommand oComandoSQL;

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
                    oComandoSQL = new SQLiteCommand(cConsultaSQL);
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
                lResultado = new List<object>() { 1, oValor };

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
        /// Ejecuta el procedimiento almacenado y devuelve una lista de resultado
        /// </summary>
        /// <param name="cProcedimiento">Procedimiento Almacenado a ejecutar</param>
        /// <param name="dicParametros">Diccionario de claves y valores parametros</param>
        /// <returns>Lista de Resultados</returns>
        public override List<object> lEjecutar_procedimiento(string cProcedimiento, Dictionary<string, object> dicParametros)
        {
            throw new NotImplementedException();
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
                __oTransaccionSQL = this.Conexion.BeginTransaction();

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
                //revertimos la transaccion
                __oTransaccionSQL.Rollback();

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



        #endregion


        #region METODOS ESPECIFICOS

        /// <summary>
        /// Devuelve la instancia de la conexion establecida
        /// </summary>
        public SQLiteConnection Conexion
        {
            get
            {
                return __oConexion;
            }
        }


        /// <summary>
        /// Ejecuta una insercion masiva desde el datatable parametro a la tabla especificada
        /// los campos y orden de los mismos deben de coincidir
        /// </summary>
        /// <param name="dtDataTableParam">DataTable origen de datos</param>
        /// <param name="oOTDdestino">Instancia de OTDbase de la tabla destino</param>
        /// <param name="bBorrarAnteriores">Si se borran previamente los datos existentes en la tabla</param>
        /// <returns>Lista de resultados List[int, object]</returns>
        public List<object> lEjecutar_bulkInsert(DataTable dtDataTableParam
                                                    , OTDbase oOTDdestino
                                                    , bool bBorrarAnteriores
                                                    )
        {
            string NOMBRE_METODO = NOMBRE_CLASE + ".lEjecutar_bulkInsert()";
            List<object> lResultado = new List<object>() { 0, NOMBRE_METODO + " No Ejecutado." };
            /*
            try
            {
                //instancias auxiliares
                string cParametros = string.Empty;

                //recorremos los campos de la tabla destino
                foreach (string cCampo in oOTDdestino.Get_dic_campos().Keys)
                {
                    //si no es autogenerado
                    if (!oOTDdestino.Get_dic_campos()[cCampo].bAutoGenerado)
                    {
                        if (cParametros == string.Empty) cParametros = "?";
                        else cParametros += ", ?";
                    }
                }

                //pasamos los parametros a la sentencia
                string cInsert = string.Format(oOTDdestino.Get_insert_basico
                                              , cParametros);


                //empezamos con la insercion masiva
                using (System.Data.Common.DbTransaction dbTrans = this.Conexion.BeginTransaction())
                {
                    using (System.Data.Common.DbCommand cmd = this.Conexion.CreateCommand())
                    {
                        //si se deben de borrar los registros anteriores
                        if (bBorrarAnteriores)
                        {
                            //reseteamos 
                            System.Data.Common.DbCommand cmdDelete = this.Conexion.CreateCommand();

                            //asignamos el comando de eliminacion
                            cmdDelete.CommandText = string.Format(oOTDdestino.Get_delete_basico
                                                , " 1 = 1"
                                                );

                            //ejecutamos la sentencia
                            cmdDelete.ExecuteNonQuery();

                        }

                        //asignamos el comando de insercion
                        cmd.CommandText = cInsert;

                        //recorremos los campos de la tabla destino
                        foreach (string cCampo in oOTDdestino.CAMPOS.Keys)
                        {
                            //si no es autogenerado
                            if(!oOTDdestino.CAMPOS[cCampo].bAutoGenerado)
                                //vamos agregando un parametro para cada campo
                                cmd.Parameters.Add(cmd.CreateParameter());
                        }
                        
                        //recorremos las filas de la tabla de origen
                        foreach (DataRow oFila in dtDataTableParam.Rows)
                        {
                            //indice para parametros a cargar
                            int nIdx = 0;

                            //luego los valores de la fila
                            foreach (object oValor in oFila.ItemArray)
                            {
                                //asignamos el valor a parametro
                                cmd.Parameters[nIdx++].Value = oValor;
                            }

                            //ejecutamos la sentencia
                            cmd.ExecuteNonQuery();
                        }
                    }

                    //confirmamos los cambios
                    dbTrans.Commit();
                }

                //resultado del metodo
                lResultado = new List<object>() { 1, "Ok" };

            }
            catch (Exception ex)
            {
                //en caso de error
                lResultado = new List<object>() { -1, NOMBRE_METODO + ": " + ex.Message, ex };
            }
             * */

            //devolvemos el resultado del metodo
            return lResultado;

        }



        #endregion

    }
}
