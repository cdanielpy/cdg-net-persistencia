using System;
using System.Collections.Generic;

using System.Text;
using Microsoft.SqlServer.Server;

namespace CdgNetPersistenciaV3.ClasesBases
{
    // <summary>
    /// Clase base para utilerias de interaccion con ConectorBase de datos relacionales
    /// </summary>
    public abstract class ConectorBase : IDisposable
    {

        //PROPIEDADES DE LA CLASE
        protected string _cServidor { get; set; }
        protected int _nPuerto { get; set; }
        protected string _cServicio { get; set; }

        protected string _cUsuario { get; set; }
        protected string _cContrasena { get; set; }
        protected string _cCatalogo { get; set; }

        protected bool _bConectado { get; set; }
        public bool bMostrarSQL { get; set; }

        protected string _cCadenaConexion { get; set; }

        /// <summary>
        /// Devuelve o establece el tiempo de espera para ejecución de comandos
        /// </summary>
        public int nTiempoComandos { get; set; }

        public const string ERROR_NO_HAY_FILAS = "NO HAY FILAS DEVUELTAS";

        /// <summary>
        /// Caracter de marca de parametros de comandos SQL
        /// </summary>
        public static char MARCADOR_PARAMETRO = ':';

        public System.Data.Common.DbConnection oConexion;

        /// <summary>
        /// Constructor de la clase
        /// </summary>
        /// <param name="cUsuarioParam">Id de Usuario de SGBD</param>
        /// <param name="cContrasenaParam">Contrasena del Usuario</param>
        /// <param name="cServidorParam">Nombre o direccion del Servidor SGBD</param>
        /// <param name="cCatalogoParam">Nombre del Catalogo de BDD</param>
        /// <param name="nPuertoParam">Puerto de escucha del servicio SGBD</param>
        /// <param name="cServicioParam">Nombre del Servicio de SGBD</param>
        public ConectorBase(string cUsuarioParam, string cContrasenaParam, string cServidorParam
                            , string cCatalogoParam, int nPuertoParam, string cServicioParam)
        {
            //asignamos los valores de los PROPIEDADES
            this._cServidor = cServidorParam;
            this._nPuerto = nPuertoParam;
            this._cServicio = cServicioParam;

            this._cUsuario = cUsuarioParam;
            this._cContrasena = cContrasenaParam;
            this._cCatalogo = cCatalogoParam;

        }

        /// <summary>
        /// Constructor de la clase
        /// </summary>
        /// <param name="cUsuarioParam">Id de Usuario de SGBD</param>
        /// <param name="cContrasenaParam">Contrasena del Usuario</param>
        /// <param name="cServidorParam">Nombre o direccion del Servidor SGBD</param>
        /// <param name="cCatalogoParam">Nombre del Catalogo de BDD</param>
        /// <param name="nPuertoParam">Puerto de escucha del servicio SGBD</param>
        /// <param name="cServicioParam">Nombre del Servicio de SGBD</param>
        public ConectorBase(string cCadenaConexionParam)
        {
            //asignamos los valores de los PROPIEDADES
            this._cCadenaConexion = cCadenaConexionParam;

            //asignamos los valores de los PROPIEDADES
            this._cServidor = string.Empty;
            this._nPuerto = 0;
            this._cServicio = string.Empty;

            this._cUsuario = string.Empty;
            this._cContrasena = string.Empty;
            this._cCatalogo = string.Empty;

        }

        /// <summary>
        /// Devuelve la representacion de la clase
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("<Acceso a BD: {0}@{1}:{2} >"
                                    , this._cCatalogo
                                    , this._cServidor
                                    , this._nPuerto
                                    );
        }

        /// <summary>
        /// Abre la conexion a la base de datos
        /// </summary>
        /// <returns>Lista de Resultados</returns>
        public abstract List<object> lConectar();

        /// <summary>
        /// Cierra la conexion activa
        /// </summary>
        /// <param name="sender">Objeto que efectua la llamada al metodo</param>
        /// <returns>Lista de Resultados</returns>
        public abstract List<object> lDesconectar(object sender);

        /// <summary>
        /// Ejecuta la sentencia sql
        /// </summary>
        /// <param name="cSentenciaSQL">Sentencia SQL a ejecutar</param>
        /// <param name="dicValores">Diccionario de claves y valores parametros</param>
        /// <returns>Lista de Resultados</returns>
        public abstract List<object> lEjecutar_sentencia(string cSentenciaSQL, Dictionary<string, object> dicValores);

        /// <summary>
        /// Ejecuta la consulta sql y devuelve un DataTable como segundo elemento
        /// </summary>
        /// <param name="cConsultaSQL">Consulta SQL a ejecutar</param>
        /// <param name="dicValores">Diccionario de claves y valores parametros</param>
        /// <returns>Lista de Resultados</returns>
        public abstract List<object> lEjecutar_consulta(string cConsultaSQL, Dictionary<string, object> dicValores);

        /// <summary>
        /// Ejecuta la consulta sql y devuelve un unico objeto valor
        /// </summary>
        /// <param name="cConsultaSQL">Consulta SQL a ejecutar</param>
        /// <param name="dicValores">Diccionario de claves y valores parametros</param>
        /// <returns>Lista de Resultados</returns>
        public abstract List<object> lEjecutar_escalar(string cConsultaSQL, Dictionary<string, object> dicValores);

        /// <summary>
        /// Ejecuta el procedimiento almacenado y devuelve una lista de resultado
        /// </summary>
        /// <param name="cProcedimiento">Procedimiento Almacenado a ejecutar</param>
        /// <param name="dicValores">Diccionario de claves y valores parametros</param>
        /// <returns>Lista de Resultados</returns>
        public abstract List<object> lEjecutar_procedimiento(string cProcedimiento, Dictionary<string, object> dicValores);

        /// <summary>
        /// Establece el metodo de Autoconfirmacion de transacciones
        /// </summary>
        /// <param name="bParam">Valor a asignar</param>
        public abstract void Set_autocommit(bool bParam);

        /// <summary>
        /// Inicia una transaccion
        /// </summary>
        /// <returns>Lista de Resultados</returns>
        public abstract List<object> lIniciar_transaccion();

        /// <summary>
        /// Confirma una transaccion activa
        /// </summary>
        /// <returns>Lista de Resultados</returns>
        public abstract List<object> lConfirmar_transaccion();

        /// <summary>
        /// Revierte una transaccion activa
        /// </summary>
        /// <returns>Lista de Resultados</returns>
        public abstract List<object> lRevertir_transaccion();

        /// <summary>
        /// Muestra el comando sql ejecutandose
        /// </summary>
        /// <param name="cSentenciaSQL">Sentencia sql a desplegar</param>
        protected void _Mostrar_SQL(string cSentenciaSQL)
        {
            //si no se deben mostrar los comandos, salimos
            if (!this.bMostrarSQL) return;

            //imprimimos el comando en la consola
            /* habilitar para clr
            SqlContext.Pipe.Send(string.Format("SQL -> \"{0}\"", cSentenciaSQL));
             * */
            Console.WriteLine(string.Format("SQL -> \"{0}\"", cSentenciaSQL));
        }

        /// <summary>
        /// Muestra el comando sql ejecutandose
        /// </summary>
        /// <param name="oComando">Instancia de Comando a ejecutar</param>
        protected void _Mostrar_SQL(System.Data.Common.DbCommand oComando)
        {
            //si no se deben mostrar los comandos, salimos
            if (!this.bMostrarSQL) return;

            //imprimimos el comando en la consola
            SqlContext.Pipe.Send(string.Format("SQL -> \"{0}\"", oComando.CommandText));
            foreach (System.Data.Common.DbParameter oParam in oComando.Parameters)
            {
                /* habilitar para clr
                 * SqlContext.Pipe.Send(string.Format("\t Parametro -> [{0}] = {1}"
                                                    , oParam.ParameterName
                                                    , oParam.Value
                                                    )
                                  );*/
                Console.WriteLine(string.Format("\t Parametro -> [{0}] = {1}"
                                                    , oParam.ParameterName
                                                    , oParam.Value
                                                    )
                                  );
            }
        }

        #region Miembros de IDisposable

        public void Dispose()
        {
            _cServidor = null;
            _nPuerto = 0;
            _cServicio = null;

            _cUsuario = null;
            _cContrasena = null;
            _cCatalogo = null;

            _bConectado = false;
            bMostrarSQL = false;

            _cCadenaConexion = null;
            nTiempoComandos = 0;

        }

        #endregion
    }

}
