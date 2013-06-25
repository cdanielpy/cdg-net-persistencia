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

        #region ATRIBUTOS

        /// <summary>
        /// Constante de mensaje de consulta sin filas devueltas como
        /// resultado
        /// </summary>
        public const string ERROR_NO_HAY_FILAS = "NO HAY FILAS DEVUELTAS";

        /// <summary>
        /// Caracter de marca de parámetros de comandos SQL
        /// </summary>
        public static char MARCADOR_PARAMETRO = ':';

        /// <summary>
        /// Almacena la instancia de la conexión establecida hacia el SGBD
        /// </summary>
        public System.Data.Common.DbConnection oConexion;


        #endregion



        #region CONSTRUCTORES

        /// <summary>
        /// Constructor de la clase
        /// </summary>
        /// <param name="cCadenaConexionParam">Cadena, completa, de conexion al sistema de base de datos</param>
        /// <see cref="http://www.connectionstrings.com/"/>
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
        /// Constructor de la clase
        /// </summary>
        /// <param name="cUsuarioParam">Id de Usuario de SGBD</param>
        /// <param name="cContrasenaParam">Contrasena del Usuario</param>
        /// <param name="cServidorParam">Nombre o direccion del Servidor SGBD</param>
        /// <param name="cCatalogoParam">Nombre del Catálogo/Esquema de Base de Datos</param>
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


        #endregion



        #region PROPIEDADES

        /// <summary>
        /// Devuelve o establece el nombre o la direccion del servidor del SBGD
        /// </summary>
        protected string _cServidor { get; set; }

        /// <summary>
        /// Devuelve o establece el puerto de escucha del servicio
        /// </summary>
        protected int _nPuerto { get; set; }

        /// <summary>
        /// Devuelve o establece el nombre del servicio del SBGD
        /// </summary>
        protected string _cServicio { get; set; }

        /// <summary>
        /// Devuelve o establece el nombre del usuario del SBGD
        /// </summary>
        protected string _cUsuario { get; set; }

        /// <summary>
        /// Devuelve o establece la contraseña del usuario
        /// </summary>
        protected string _cContrasena { get; set; }

        /// <summary>
        /// Devuelve o establece el nombre del Catálogo/Esquema de Base de Datos
        /// </summary>
        protected string _cCatalogo { get; set; }

        /// <summary>
        /// Devuelve o establece el marcador de la conexión abierta
        /// </summary>
        protected bool _bConectado { get; set; }

        /// <summary>
        /// Devuelve o establece la cadena de conxión utilizada
        /// </summary>
        protected string _cCadenaConexion { get; set; }

        /// <summary>
        /// Devuelve o establece el valor de marcador para desplegar las
        /// cadenas de los comandos ejecutados en el servidor
        /// </summary>
        public bool bMostrarSQL { get; set; }

        /// <summary>
        /// Devuelve o establece el tiempo de espera para ejecución de comandos
        /// </summary>
        public int nTiempoComandos { get; set; }


        #endregion



        #region METODOS

        /// <summary>
        /// Abre la conexion a la base de datos
        /// </summary>
        /// <returns>Lista de Resultados</returns>
        public abstract List<object> lConectar();

        /// <summary>
        /// Cierra la conexión activa
        /// </summary>
        /// <param name="sender">Objeto que efectúa la llamada al método</param>
        /// <returns>Lista de Resultados</returns>
        public abstract List<object> lDesconectar(object sender);

        /// <summary>
        /// Ejecuta la sentencia sql parámetro
        /// </summary>
        /// <param name="cSentenciaSQL">Sentencia SQL a ejecutar</param>
        /// <param name="dicValores">Diccionario de claves y valores parámetros de la sentencia</param>
        /// <returns>Lista de Resultados</returns>
        public abstract List<object> lEjecutar_sentencia(string cSentenciaSQL, Dictionary<string, object> dicValores);

        /// <summary>
        /// Ejecuta la consulta sql y devuelve un DataTable como segundo elemento
        /// de la lista de resultados
        /// </summary>
        /// <param name="cConsultaSQL">Consulta SQL a ejecutar</param>
        /// <param name="dicValores">Diccionario de claves y valores parámetros de la consulta</param>
        /// <returns>Lista de Resultados</returns>
        public abstract List<object> lEjecutar_consulta(string cConsultaSQL, Dictionary<string, object> dicValores);

        /// <summary>
        /// Ejecuta la consulta sql y devuelve un unico objeto, que representa al valor
        /// de la primera columna de la primera fila devuelta por la consulta
        /// </summary>
        /// <param name="cConsultaSQL">Consulta SQL a ejecutar</param>
        /// <param name="dicValores">Diccionario de claves y valores parámetros de la consulta</param>
        /// <returns>Lista de Resultados</returns>
        public abstract List<object> lEjecutar_escalar(string cConsultaSQL, Dictionary<string, object> dicValores);

        /// <summary>
        /// Ejecuta el procedimiento almacenado y devuelve una lista de resultados
        /// </summary>
        /// <param name="cProcedimiento">Procedimiento almacenado a ejecutar</param>
        /// <param name="dicValores">Diccionario de claves y valores parámetros del procedimiento</param>
        /// <returns>Lista de Resultados</returns>
        public abstract List<object> lEjecutar_procedimiento(string cProcedimiento, Dictionary<string, object> dicValores);

        /// <summary>
        /// Establece el método de Autoconfirmación de transacciones
        /// </summary>
        /// <param name="bParam">Valor a asignar
        /// Si es {{{true}}} todos los comandos se confirmarán apenas hayan terminados su ejecución 
        /// en forma exitosa, sino, se ejecutará un rollback automático
        /// </param>
        public abstract void Set_autocommit(bool bParam);

        /// <summary>
        /// Inicia una transacción de base de datos
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
        /// Muestra el comando sql parámetro
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
        /// <param name="oComando">Instancia de Comando que contiene la sentencia sql a desplegar</param>
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

        #endregion



        #region SOBREESCRITOS

        /// <summary>
        /// Devuelve la representación de cadena de la clase
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

        #region Miembros de IDisposable

        /// <summary>
        /// Ejecuta la liberación de todos los recursos recursos creados por la instancia
        /// </summary>
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

        #endregion


    }

}
