using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System;
using CdgNetPersistenciaV3_5.ClasesBases;
using System.Reflection;
using CdgNetPersistenciaV3_5.Atributos;
using System.Text;

/**
 * Autor :  Cristian Daniel Garay Sosa
 * Fecha :  15/10/2012 (v3)
 * Comentarios:
 *          clase base de administracion de interaccion de la aplicacion 
 *          con los datos de una tabla y sus relaciones
 * 
 * Autor :  Cristian Daniel Garay Sosa
 * Fecha : 26/03/2015
 * Comentarios :
 *          Se reemplazan los resultados de los metodos de List<object> a object[]
 *          En el constructor se reemplaza el parametro de instancia del OTD a adminstrar por su Tipo (Type)
 *          En las condiciones "WHERE" que se forman, por defecto se añade la primera condicion " 1 = 1 "
 * 
* **/

namespace CdgNetPersistenciaV3_5.ClasesBases
{
    /// <summary>
    /// Clase base de estructura de un administrador de Tabla
    /// </summary>
    public abstract class ADMbase
    {

        #region CAMPOS(Fields) DE LA CLASE

        public static string NOMBRE_CLASE = "ADMbase";

        /// <summary>
        /// Marcador estándar de nombres de parámetros de comandos SQL
        /// </summary>
        public const char MARCADOR_PARAMETRO_ESTANDAR = '?';

        /// <summary>
        /// Fecha de marca para valores de campos nulos del tipo DateTime
        /// Deprecado
        /// </summary>
        public static DateTime FECHA_MARCA_NULA = DateTime.MinValue;

        /// <summary>
        /// Almacena el marcador de parámetros SQL
        /// </summary>
        public char cMarcaParametro = MARCADOR_PARAMETRO_ESTANDAR;

        /// <summary>
        /// Almacena la instancia de utilería de conexión a la base de datos
        /// </summary>
        protected ConectorBase _oUtileriaDeConexion;

        /// <summary>
        /// Almacena la lista de nombres de campos físicos de la tabla
        /// en una lista separada por comas
        /// </summary>
        protected string _cListaCamposTabla;

        /// <summary>
        /// Almacena el tipo de de la clase extendida de OTDbase
        /// </summary>
        private Type __tTipoOTD;

        // cadenas de sentencias bases
        private const string __SELECT_SQL = "SELECT {0} FROM {1} AS {2} ";
        private const string __INSERT_SQL = "INSERT INTO {0} ({1}) VALUES ({2}) ";
        private const string __UPDATE_SQL = "UPDATE {0} SET {1} WHERE {2} ";
        private const string __DELETE_SQL = "DELETE FROM {0} WHERE {1} ";

        /// Almacen de cadenas DML básicas
        private string __cSelect_sql;
        private string __cInsert_sql;
        private string __cUpdate_sql;
        private string __cDelete_sql;
        private string __cWhereSugerido;

        /// <summary>
        /// Nombre de la tabla administrada
        /// </summary>
        private string __cNombreTabla;

        /// <summary>
        /// Un alias para el nombre de la tabla
        /// </summary>
        protected string _cAliasTabla;

        /// <summary>
        /// Condicion utilizada para limite de filas a recuperar en un select
        /// </summary>
        protected string _cRestriccionFilas;

        /// <summary>
        /// Almacena el tipo de SGBD administrado
        /// </summary>
        protected CdgNetPersistenciaV3_5.Atributos.Tabla.SGBD _eSGBD;

        //diccionarios para datos de campos de los OTD
        Dictionary<string, Campo> __dicCampos;
        Dictionary<string, PropertyInfo> __dicPropiedades;

        #endregion



        #region CONSTRUCTORES

        /// <summary>
        /// Contructor de la clase
        /// </summary>
        /// <param name="oConectorBase">Instancia de utilería de interacción con la base de datos</param>
        /// <param name="tTipoOtdextendido">Tipo de la clase heredada de OTDbase que sera administrada por esta</param>
        protected ADMbase(ConectorBase oConectorBase, Type tTipoOtdextendido)
        {
            //tomamos las instancias parametro
            _oUtileriaDeConexion = oConectorBase;
            __tTipoOTD = tTipoOtdextendido;

            //instanciamos los nuevos diccionarios
            __dicCampos = new Dictionary<string, Campo>();
            __dicPropiedades = new Dictionary<string, PropertyInfo>();

            //tomamos los datos de la tabla que representa el OTD administrado
            _Set_tabla();

        }


        #endregion



        #region GETTERS Y SETTERS

        /// <summary>
        /// Devuelve el nombre de la tabla administrada
        /// </summary>
        public string NombreTabla
        {
            get { return __cNombreTabla; }
        }

        /// <summary>
        /// Devuelve o establece el comando de seleccion de datos de la tabla
        /// </summary>
        public virtual string Select_sql
        {
            get
            {
                if (__cSelect_sql == null || __cSelect_sql == string.Empty)
                    __cSelect_sql = string.Format(__SELECT_SQL, _cListaCamposTabla, __cNombreTabla, _cAliasTabla);
                return __cSelect_sql;
            }
            set
            { __cSelect_sql = value; }

        }

        /// <summary>
        /// Devuelve o establece el comando de inserción de datos a la tabla
        /// </summary>
        public virtual string Insert_sql
        {
            get
            {
                if (__cInsert_sql == null || __cInsert_sql == string.Empty)
                    __SetInsertSql();
                return __cInsert_sql;
            }
            set { __cInsert_sql = value; }
        }

        /// <summary>
        /// Devuelve o establece el comando de actualización de datos de la tabla
        /// </summary>
        public virtual string Update_sql
        {
            get
            {
                if (__cUpdate_sql == null || __cUpdate_sql == string.Empty)
                    __SetUpdateSql();
                return __cUpdate_sql;
            }
            set { __cUpdate_sql = value; }
        }

        /// <summary>
        /// Devuelve o establece el comando de eliminación de datos de la tabla
        /// </summary>
        public virtual string Delete_sql
        {
            get
            {
                if (__cDelete_sql == null || __cDelete_sql == string.Empty)
                    __cDelete_sql = string.Format(__DELETE_SQL, __cNombreTabla, this.WhereSugerido);
                return __cDelete_sql;
            }
            set
            { __cDelete_sql = value; }
        }

        /// <summary>
        /// Devuelve las condiciones de filtrado de registros sugerida
        /// en base a la estructura especificada de la tabla
        /// </summary>
        public virtual string WhereSugerido
        {
            get
            {
                if (__cWhereSugerido == null || __cWhereSugerido == string.Empty)
                    __SetWhereSugerido();
                return __cWhereSugerido;
            }
            set { __cWhereSugerido = value; }
        }

        /// <summary>
        /// Devuelve el diccionario de nombres de campos y sus instancias
        /// </summary>
        protected virtual Dictionary<string, Campo> DiccionarioCampos
        {
            get
            {
                return __dicCampos;
            }
        }

        #endregion



        #region METODOS DML

        /// <summary>
        /// Inserta un nuevo registro en la tabla a partir de la instancia parámetro
        /// </summary>
        /// <param name="oOTDbase">Instancia de la clase extendida de OTDbase de la cual se obtienen los datos
        /// del registro a insertar</param>
        /// <returns>Arreglo de resultado [int, resultado]</returns>
        public abstract object[] aAgregar(OTDbase oOTDbase);

        /// <summary>
        /// Inserta un nuevo registro en la tabla a partir de la instancia parámetro
        /// </summary>
        /// <typeparam name="T">Clase extendida de OTDbase de la cual se obtienen los datos
        /// del registro a insertar</typeparam>
        /// <param name="oOTDbase">Instancia de la clase que contiene los datos a insertar</param>
        /// <returns>Arreglo de resultado [int, resultado]</returns>
        public virtual object[] aAgregar<T>(OTDbase oOTDbase)
        {

            string NOMBRE_METODO = NOMBRE_CLASE + ".aAgregar()";

            // lista con el resultado del metodo 
            object[] aResultado = new object[] { 0, NOMBRE_METODO + " No Ejecutado!" };

            try
            {
                //si el tipo es del mismo que se esta administrando
                if (__tTipoOTD == typeof(T))
                {
                    //ejecutamos la sentencia de actualizacion
                    aResultado = _oUtileriaDeConexion.aEjecutar_sentencia(Insert_sql, this.ParametrosDML(oOTDbase, true));

                    //si se inserto correctamente, recuperamos la misma instancia desde la base de datos
                    if ((int)aResultado[0] == 1) return this.aGet_elemento<T>(oOTDbase);

                }
                else
                {
                    //sino, excepcion
                    throw new NotSupportedException(string.Format("El tipo <{0}> especificado no es del mismo tipo {1} administrado!"
                                                                    , typeof(T), __tTipoOTD)
                                                    );
                }
            }
            catch (Exception ex)
            {
                //en caso de error, establecemos el resultado como corresponde
                aResultado[0] = -1;
                aResultado[1] = NOMBRE_METODO + " Error: " + ex.Message;
            }

            //devolvemos el resultado del metodo
            return aResultado;

        }

        /// <summary>
        /// Actualiza el registro en la tabla a partir de la instancia parámetro.
        /// La instancia, a su vez, representa al registro a actualizar
        /// </summary>
        /// <param name="oOTDbase">Instancia de la clase que representa a un registro</param>
        /// <returns>Arreglo de resultado [int, resultado]</returns>
        public abstract object[] aActualizar(OTDbase oOTDbase);

        /// <summary>
        /// Actualiza el registro en la tabla a partir de la instancia parámetro.
        /// La instancia, a su vez, representa al registro a actualizar
        /// </summary>
        /// <typeparam name="T">Clase extendida de OTDbase de la cual se obtienen los datos
        /// del registro a actualizar y sus nuevos valores</typeparam>
        /// <param name="oOTDbase">Instancia de la clase que representa a un registro</param>
        /// <remarks>Para utilizar este metodo la instancia debe tener al menos un campo Identificador</remarks>
        /// <returns>Arreglo de resultado [int, resultado]</returns>
        public virtual object[] aActualizar<T>(OTDbase oOTDbase)
        {
            string NOMBRE_METODO = NOMBRE_CLASE + ".aActualizar()";

            // lista con el resultado del metodo 
            object[] aResultado = new object[] { 0, NOMBRE_METODO + " No Ejecutado!" };

            try
            {
                //si el tipo es del mismo que se esta administrando
                if (__tTipoOTD == typeof(T))
                {
                    //ejecutamos la sentencia de actualizacion
                    aResultado = _oUtileriaDeConexion.aEjecutar_sentencia(Update_sql, this.ParametrosDML(oOTDbase, true));

                    //si se inserto correctamente, recuperamos la misma instancia desde la base de datos
                    if ((int)aResultado[0] == 1) return this.aGet_elemento<T>(oOTDbase);

                }
                else
                {
                    //sino, excepcion
                    throw new NotSupportedException(string.Format("El tipo <{0}> especificado no es del mismo tipo {1} administrado!"
                                                                    , typeof(T), __tTipoOTD
                                                                 )
                                                    );
                }
            }
            catch (Exception ex)
            {
                //en caso de error, establecemos el resultado como corresponde
                aResultado[0] = -1;
                aResultado[1] = NOMBRE_METODO + " Error: " + ex.Message;
            }

            //devolvemos el resultado del metodo
            return aResultado;

        }

        /// <summary>
        /// Elimina el registro de la tabla representado por la instancia parámetro
        /// </summary>
        /// <param name="oOTDbase">Instancia de la clase que representa a un registro</param>
        /// <returns>Arreglo de resultado [int, resultado]</returns>
        public abstract object[] aEliminar(OTDbase oOTDbase);

        /// <summary>
        /// Elimina el registro de la tabla representado por la instancia parámetro
        /// </summary>
        /// <typeparam name="T">Clase extendida de OTDbase de la cual se obtienen los datos
        /// del registro a eliminar</typeparam>
        /// <param name="oOTDbase">Instancia de la clase que representa al registro a eliminar</param>
        /// <remarks>Para utilizar este metodo la instancia debe tener al menos un campo Identificador</remarks>
        /// <returns>Arreglo de resultado [int, resultado]</returns>
        public virtual object[] aEliminar<T>(OTDbase oOTDbase)
        {
            string NOMBRE_METODO = NOMBRE_CLASE + ".aEliminar()";

            // lista con el resultado del metodo 
            object[] aResultado = new object[] { 0, NOMBRE_METODO + " No Ejecutado!" };

            try
            {
                //si el tipo es del mismo que se esta administrando
                if (__tTipoOTD == typeof(T))
                {
                    //ejecutamos la sentencia de actualizacion
                    return _oUtileriaDeConexion.aEjecutar_sentencia(Delete_sql, this.ParametrosDML(oOTDbase, true));
                }
                else
                {
                    //sino, excepcion
                    throw new NotSupportedException(string.Format("El tipo <{0}> especificado no es del mismo tipo {1} administrado!"
                                                                    , typeof(T), __tTipoOTD
                                                                 )
                                                    );
                }
            }
            catch (Exception ex)
            {
                //en caso de error, establecemos el resultado como corresponde
                aResultado[0] = -1;
                aResultado[1] = NOMBRE_METODO + " Error: " + ex.Message;
            }

            //devolvemos el resultado del metodo
            return aResultado;
        }

        /// <summary>
        /// Obtiene una única instancia que coincide con el ID de la instancia parámetro
        /// , o, en su defecto, por los demas valores de atributos no nulos
        /// </summary>
        /// <param name="oOTDbase">Instancia de la clase que representa a un registro</param>
        /// <returns>Arreglo de resultado [int, resultado]</returns>
        public abstract object[] aGet_elemento(OTDbase oOTDbase);

        /// <summary>
        /// Obtiene una única instancia que coincide con el ID de la instancia parámetro
        /// , o, en su defecto, por los demas valores de atributos no nulos
        /// </summary>
        /// <typeparam name="T">Clase extendida de OTDbase a la cual se transfieren los datos
        /// del registro</typeparam>
        /// <param name="oOTDbase">Instancia de la clase que representa a un registro</param>
        /// <returns>Arreglo de resultado [int, resultado]</returns>
        public virtual object[] aGet_elemento<T>(OTDbase oOTDbuscado)
        {
            string NOMBRE_METODO = NOMBRE_CLASE + ".aGet_elemento()";

            //lista para  resultado del metodo 
            object[] aResultado = new object[] { 0, NOMBRE_METODO + " No Ejecutado!" };

            try
            {
                //si el tipo es del mismo que se esta administrando
                if (__tTipoOTD == typeof(T))
                {
                    // variable para el filtrado
                    string cFiltroWhere = " WHERE 1 = 1 ";

                    //recorremos los campos de la tabla referenciada
                    foreach (string cNombreCampo in __dicPropiedades.Keys)
                    {
                        //llamamos al metodo getter de la propiedad actual
                        var oValor = __tTipoOTD.InvokeMember(__dicPropiedades[cNombreCampo].Name
                                                            , BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.Public
                                                            , null, oOTDbuscado
                                                            , new object[] { }
                                                            );

                        //si no es nulo
                        if (oValor != null)
                        {
                            //si es un identificador y no esta vacio
                            if (__dicCampos[cNombreCampo].Identificador
                                    && oValor.ToString() != "0"
                                    && oValor.ToString() != string.Empty
                                )
                            {
                                //si es de tipo long
                                if (oValor.GetType() == typeof(long))
                                    //si es igual al valor minimo de este tipo, lo saltamos
                                    if (((long)oValor) == long.MinValue) continue;

                                //lo agregamos a la condicion de filtrado
                                cFiltroWhere += string.Format(" AND {0} = {1}{2} "
                                                                , cNombreCampo
                                                                , this.cMarcaParametro
                                                                , __dicCampos[cNombreCampo].Nombre.ToLower()
                                                                );
                            }
                            else if (!__dicCampos[cNombreCampo].Identificador)
                            {
                                //si es de tipo DateTime
                                if (oValor.GetType() == typeof(DateTime))
                                    //si es igual al valor minimo de este tipo, lo saltamos
                                    if (((DateTime)oValor).Date == ADMbase.FECHA_MARCA_NULA) continue;

                                //si es de tipo long
                                if (oValor.GetType() == typeof(long))
                                    //si es igual al valor minimo de este tipo, lo saltamos
                                    if (((long)oValor) == long.MinValue) continue;

                                //si es de tipo int
                                if (oValor.GetType() == typeof(int))
                                    //si es igual al valor minimo de este tipo, lo saltamos
                                    if (((int)oValor) == int.MinValue) continue;

                                //si es de tipo double
                                if (oValor.GetType() == typeof(double))
                                    //si es igual al valor minimo de este tipo, lo saltamos
                                    if (((double)oValor) == double.MinValue) continue;

                                //si es de tipo decimal
                                if (oValor.GetType() == typeof(decimal))
                                    //si es igual al valor minimo de este tipo, lo saltamos
                                    if (((decimal)oValor) == decimal.MinValue) continue;

                                //si es de tipo float
                                if (oValor.GetType() == typeof(float))
                                    //si es igual al valor minimo de este tipo, lo saltamos
                                    if (((float)oValor) == float.MinValue) continue;

                                //lo agregamos a la condicion de filtrado
                                cFiltroWhere += string.Format(" AND {0} = {1}{2} "
                                                                , cNombreCampo
                                                                , this.cMarcaParametro
                                                                , __dicCampos[cNombreCampo].Nombre.ToLower()
                                                                );
                            }

                        }
                    }

                    //invocamos al metodo de recuperacion de registros desde la tabla
                    aResultado = this.aGet_elementos(cFiltroWhere, this.ParametrosDML(oOTDbuscado, false));

                    //si se ejecuto correctamente, devolvemos solo el primer elemento de la lista de instancias devueltas
                    if ((int)aResultado[0] == 1) return new object[] { 1, (aResultado[1] as List<T>)[0]};

                }
                else
                {
                    //sino, excepcion
                    throw new NotSupportedException(string.Format("El tipo <{0}> especificado no es del mismo tipo {1} administrado!"
                                                                    , typeof(T), __tTipoOTD
                                                                 )
                                                    );
                }
            }
            catch (Exception ex)
            {
                //en caso de error, establecemos el resultado como corresponde
                aResultado[0] = -1;
                aResultado[1] = NOMBRE_METODO + " Error: " + ex.Message;
            }

            //devolvemos el resultado del metodo
            return aResultado;

        }

        /// <summary>
        /// Obtiene una lista de instancias que satisfacen 
        /// la condicion de filtrado desde la base de datos
        /// </summary>
        /// <param name="cFiltroWhere">Condicion de filtrado de registros</param>
        /// <returns>Arreglo de Resultados [int, object]</returns>
        public abstract object[] aGet_elementos(string cFiltroWhere);

        /// <summary>
        /// Obtiene una lista de instancias que satisfacen 
        /// la condicion de filtrado desde la base de datos
        /// </summary>
        /// <param name="cFiltroWhere">Condicion de filtrado de registros</param>
        /// <param name="nCantidadRegistros">Cantidad registros a recuperar desde la base de datos</param>
        /// <returns>Arreglo de Resultados [int, object]</returns>
        public abstract object[] aGet_elementos(string cFiltroWhere, int nCantidadRegistros);

        /// <summary>
        /// Obtiene una lista de instancias que satisfacen 
        /// la condicion de filtrado desde la base de datos
        /// </summary>
        /// <param name="cFiltroWhere">Condicion de filtrado de registros</param>
        /// <param name="dicParametros">Diccionario de claves y valores de parámetros de la sentencia</param>
        /// <returns>Arreglo de Resultados [int, object]</returns>
        public abstract object[] aGet_elementos(string cFiltroWhere, Dictionary<string, object> dicParametros);
        
        /// <summary>
        /// Obtiene una lista de instancias que satisfacen 
        /// la condicion de filtrado desde la base de datos
        /// </summary>
        /// <param name="cFiltroWhere">Condicion de filtrado de registros</param>
        /// <param name="dicParametros">Diccionario de claves y valores de parámetros de la sentencia</param>
        /// <param name="nCantidadRegistros">Cantidad registros a recuperar desde la base de datos</param>
        /// <returns>Arreglo de Resultados [int, object]</returns>
        public abstract object[] aGet_elementos(string cFiltroWhere, Dictionary<string, object> dicParametros, int nCantidadRegistros);

        /// <summary>
        /// Obtiene una lista de instancias que satisfacen 
        /// la condicion de filtrado desde la base de datos
        /// </summary>
        /// <typeparam name="T">Clase extendida de OTDbase a la cual se transfieren los datos 
        /// de registros</typeparam>
        /// <param name="cFiltroWhere">Condicion de filtrado de registros</param>
        /// <returns>Arreglo de Resultados [int, object]</returns>
        public virtual object[] aGet_elementos<T>(string cFiltroWhere)
        {
            return this.aGet_elementos<T>(cFiltroWhere, new Dictionary<string,object>(), -1);
        }

        /// <summary>
        /// Obtiene una lista de instancias que satisfacen 
        /// la condicion de filtrado desde la base de datos
        /// </summary>
        /// <typeparam name="T">Clase extendida de OTDbase a la cual se transfieren los datos 
        /// de registros</typeparam>
        /// <param name="cFiltroWhere">Condicion de filtrado de registros</param>
        /// <param name="nCantidadRegistros">Cantidad registros a recuperar desde la base de datos</param>
        /// <returns>Arreglo de Resultados [int, object]</returns>
        public virtual object[] aGet_elementos<T>(string cFiltroWhere, int nCantidadRegistros)
        {
            return this.aGet_elementos<T>(cFiltroWhere, new Dictionary<string, object>(), nCantidadRegistros);
        }

        /// <summary>
        /// Obtiene una lista de instancias que satisfacen 
        /// la condicion de filtrado desde la base de datos
        /// </summary>
        /// <typeparam name="T">Clase extendida de OTDbase a la cual se transfieren los datos 
        /// de registros</typeparam>
        /// <param name="cFiltroWhere">Condicion de filtrado de registros</param>
        /// <param name="dicParametros">Diccionario de claves y valores de parámetros de la sentencia</param>
        /// <returns>Arreglo de Resultados [int, object]</returns>
        public virtual object[] aGet_elementos<T>(string cFiltroWhere, Dictionary<string, object> dicParametros)
        {
            return this.aGet_elementos<T>(cFiltroWhere, dicParametros, -1);
        }

        /// <summary>
        /// Obtiene una lista de instancias que satisfacen 
        /// la condicion de filtrado desde la base de datos
        /// </summary>
        /// <typeparam name="T">Clase extendida de OTDbase a la cual se transfieren los datos 
        /// de registros</typeparam>
        /// <param name="cFiltroWhere">Condicion de filtrado de registros</param>
        /// <param name="dicParametros">Diccionario de claves y valores de parámetros de la sentencia</param>
        /// <param name="nCantidadRegistros">Cantidad registros a recuperar desde la base de datos</param>
        /// <returns>Arreglo de Resultados [int, object]</returns>
        public virtual object[] aGet_elementos<T>(string cFiltroWhere, Dictionary<string, object> dicParametros, int nCantidadRegistros)
        {
            string NOMBRE_METODO = NOMBRE_CLASE + ".aGet_elementos()";

            //una lista para le resultado del metodo 
            object[] aResultado = new object[] { 0, NOMBRE_METODO + " No Ejecutado!" };

            try
            {
                if(nCantidadRegistros <= 0)
                    //invocamos al metodo de ejecucion de la consulta
                    aResultado = _oUtileriaDeConexion.aEjecutar_consulta(Select_sql + cFiltroWhere, dicParametros);
                else
                    if (_eSGBD == Tabla.SGBD.SQL_SERVER)
                        aResultado = _oUtileriaDeConexion.aEjecutar_consulta(string.Format(_cRestriccionFilas, nCantidadRegistros) + Select_sql.ToUpper().Replace("SELECT", "") + cFiltroWhere, dicParametros);
                    else
                        aResultado = _oUtileriaDeConexion.aEjecutar_consulta(Select_sql + cFiltroWhere + string.Format(_cRestriccionFilas, nCantidadRegistros), dicParametros);

                //si se ejecuto correctamente,invocamos al metodo de conversion a coleccion de OTDs y establecemos el resultado
                if ((int)aResultado[0] == 1) return new object[] { 1, lSet_registros<T>((aResultado[1] as DataTable).Rows) };

            }
            catch (Exception ex)
            {
                //en caso de error, establecemos el resultado como corresponde
                aResultado[0] = -1;
                aResultado[1] = NOMBRE_METODO + " Error: " + ex.Message;
            }

            //devolvemos el resultado del metodo
            return aResultado;

        }



        /// <summary>
        /// Ejecuta los comandos de eliminacion y creacion de la tabla
        /// administrada
        /// </summary>
        /// <param name="cStringDrop">Cadena de DROPING de la tabla o string.Empty
        /// para cadena por defecto</param>
        /// <param name="cStringCreate">Cadena de CREATING de la tabla o string.Empty
        /// para cadena por defecto</param>
        /// <returns>Lista de Resultados[int, object]</returns>
        /*
        public virtual object[] aRecrear_tabla(string cStringDrop, string cStringCreate)
        {

            string NOMBRE_METODO = NOMBRE_CLASE + ".lRecrear_tabla()";

            //lista de resultado
            object[] aResultado = new object[];

            try
            {
                //si se recibe la cadena de eliminacion de la tabla
                if (cStringDrop != null
                    && cStringDrop != string.Empty)
                    //la ejecutamos
                    aResultado = _oConector.lEjecutar_sentencia(cStringDrop, new Dictionary<string, object>());
                else
                    //sino, la sentencia por defecto
                    aResultado = _oConector.lEjecutar_sentencia(_oTabla.Get_drop_tabla, new Dictionary<string, object>());

                //si se ejecuto correctamente
                if ((int)aResultado[0] == 1)
                {
                    //si se recibe la cadena de creacion de la tabla
                    if (cStringCreate != null
                        && cStringCreate != string.Empty)
                        //la ejecutamos
                        aResultado = _oConector.lEjecutar_sentencia(cStringCreate, new Dictionary<string, object>());
                    else
                        //sino, la sentencia por defecto
                        aResultado = _oConector.lEjecutar_sentencia(_oTabla.Get_create_tabla, new Dictionary<string, object>());
                }

            }
            catch (Exception ex)
            {
                //en caso de error
                aResultado[0] = -1;
                aResultado[1] = NOMBRE_METODO + " Error: " + ex.Message;
            }

            //devolvemos el resultado del metodo
            return aResultado;

        }
        */


        #endregion



        #region METODOS DE PASO OTD -> DATOS - DATOS -> OTD

        /// <summary>
        /// Ejecuta la instanciación y asignación del OTD administrado por la clase
        /// </summary>
        /// <typeparam name="T">Clase de OTDbase extendida a los que seran 'convertidas' las filas</typeparam>
        /// <param name="aFilasParam">Arreglo de filas de un DataTable</param>
        /// <see cref="http://www.csharp-examples.net/reflection-examples/" />
        public List<T> lSet_registros<T>(DataRowCollection aFilasParam)
        {
            //reinstanciamos la lista a cargar
            List<T> lInstancias = new List<T>();

            //si el tipo es del mismo que se esta administrando
            if (__tTipoOTD == typeof(T))
            {
                //recorremos las filas de tabla
                foreach (DataRow dr in aFilasParam)
                {
                    //creamos una instancia del OTD extendido
                    object oInstanciaOTD = Activator.CreateInstance<T>();

                    //recorremos las propiedades a asignar
                    foreach (string cNombreCampo in __dicPropiedades.Keys)
                    {
                        //si no es un valor nulo
                        if (dr[cNombreCampo].GetType() != typeof(DBNull))
                            //setteamos la propiedad actual haciendo la conversion de tipos dinamicamente
                            __tTipoOTD.GetProperty(__dicPropiedades[cNombreCampo].Name)
                                .SetValue(oInstanciaOTD
                                            , Convert.ChangeType(dr[cNombreCampo]
                                                                , __dicPropiedades[cNombreCampo].PropertyType
                                                                , null
                                                                )
                                           , null
                                          );
  
                            //llamamos al metodo setter actual
                            /*__tTipoOTD.InvokeMember(__dicPropiedades[cNombreCampo].Name
                                                    , BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.Public
                                                    , null, oInstanciaOTD
                                                    , new object[] { dr[cNombreCampo] }
                                                    );*/
                    }

                    //agregamos la instancia nueva a la coleccion
                    lInstancias.Add((T)oInstanciaOTD);

                }
            }
            else
            {
                //sino, excepcion
                throw new NotSupportedException(string.Format("El tipo <{0}> especificado no es del mismo tipo {1} administrado!"
                                                                , typeof(T), __tTipoOTD
                                                             )
                                                );
            }

            //devolvemos el resultado del metodo
            return lInstancias;

        }


        #endregion



        #region GENERADORES DE CODIGO DML

        /// <summary>
        /// Establece los datos de la estructura física de la tabla en base 
        /// a la instancia del OTDbase parámetro
        /// </summary>
        protected void _Set_tabla()
        {
            //si aun no esta instanciado
            if (__cNombreTabla == null)
            {
                //recorremos sus atributos
                //http://msdn.microsoft.com/es-es/library/z919e8tw%28v=vs.80%29.aspx
                foreach (var oAtributo in __tTipoOTD.GetCustomAttributes(true))
                {
                    //si es del tipo Tabla
                    if (oAtributo is Tabla)
                    {
                        //tomamos el nombre de la tabla asignada
                        __cNombreTabla = (oAtributo as Tabla).Nombre;
                        _cAliasTabla = __cNombreTabla.Substring(0, 1);
                        _eSGBD = (oAtributo as Tabla).TipoSGBD;

                        //evaluamos el tipo de marcador de parámetro segun el SGBD
                        switch (_eSGBD)
                        {
                            case Tabla.SGBD.SQL_SERVER:
                                this.cMarcaParametro = SQLServerUtiles.MARCADOR_PARAMETRO;
                                _cRestriccionFilas = "SELECT TOP {0} ";
                                break;
                            
                            case Tabla.SGBD.ORACLE:
                                this.cMarcaParametro = OracleUtiles.MARCADOR_PARAMETRO;
                                _cRestriccionFilas = " ROWNUM < {0} + 1";
                                break;

                            case Tabla.SGBD.SQLITE:
                                this.cMarcaParametro = SQLiteUtiles.MARCADOR_PARAMETRO;
                                _cRestriccionFilas = " LIMIT {0}";
                                break;

                            case Tabla.SGBD.MYSQL:
                                this.cMarcaParametro = MySqlUtiles.MARCADOR_PARAMETRO;
                                _cRestriccionFilas = " LIMIT {0}";
                                break;
                        }

                        //salimos del bucle
                        break;

                    }
                }

                //tomamos los nombres datos de campos
                _Set_campos();

                //invocamos a los metodos generadores de DML
                _SetListaCamposTabla();

            }

        }



        /**
         * GENERADORES PARA SOBREESCRIBIR LOS ACTUALES DESDE LA CLASE EXTENDIDA
         * **/


        /// <summary>
        /// Establece los datos de la tabla fisica administrada
        /// </summary>
        protected void _Set_campos() {
            
            //tomamos los campos de la clase
            PropertyInfo[] aPropiedades = __tTipoOTD.GetProperties(BindingFlags.GetProperty
                                                                    | BindingFlags.Instance
                                                                    | BindingFlags.Public
                                                                    );

            //una variable para el indice de campos
            var nIdx = 0;

            //recorremos los metodos de la clase actual
            foreach (var oPropiedad in aPropiedades)
            {
                //http://msdn.microsoft.com/es-es/library/z919e8tw%28v=vs.80%29.aspx
                //recorremos sus atributos
                foreach (object atributo in oPropiedad.GetCustomAttributes(true))
                {
                    //si es del tipo Campo
                    if (atributo is Campo)
                    {
                        //lo tomamos 
                        Campo oCampo = (Campo)atributo;

                        //agregamos la propiedad al diccionario
                        __dicPropiedades.Add(oCampo.Nombre, oPropiedad);

                        //le asignamos su posicion si no lo tiene aun
                        if (oCampo.Indice == 0) oCampo.Indice = nIdx;

                        //le asignamos su tipo
                        oCampo.Tipo = oPropiedad.PropertyType;

                        //si el campo NO esta en en el dicionario ya
                        if (!__dicCampos.ContainsKey(oCampo.Nombre))
                            //tomamos la instancia correspondiente y la agregamos al diccionario
                            __dicCampos.Add(oCampo.Nombre, oCampo);

                        //incrementamos el contador
                        nIdx++;
                    }
                }
            }
        
        }
        
        /// <summary>
        /// Devuelve una cadena con los nombres de los campos con alias de la incluído
        /// TABLA.CAMPO0, TABLA.CAMPO1, TABLA.CAMPO2, ...
        /// </summary>
        protected void _SetListaCamposTabla()
        {
            //pasamos los nombres de campos a una lista auxiliar
            var lCampos = new List<string>();
            foreach(string cCampo in __dicCampos.Keys) lCampos.Add(cCampo);

            //formamos la cadena correspondiente
            this._cListaCamposTabla = _cAliasTabla
                                        + "." + string.Join(", " + _cAliasTabla
                                        + ".", lCampos.ToArray()
                                        );
        }

        /**
         * GENERADORES SOBRE PEDIDO INTERNOS DESDE LOS ACCESORES
         * **/

        /// <summary>
        /// Establece la sentencia sql base de inserción de registros en la tabla administrada
        /// </summary>
        private void __SetInsertSql()
        {
            //concatenamos la lista de campos NO autogenerados
            StringBuilder sbCampos = new StringBuilder();
            StringBuilder sbValores = new StringBuilder();
            int c = 1;

            //recorremos los campos de la tabla
            foreach (Campo oCampo in __dicCampos.Values)
            {
                //si no es generado por el propio SGBD
                if (!oCampo.AutoGenerado)
                {
                    if (sbCampos.ToString().Equals(string.Empty))
                        sbCampos.Append(oCampo.Nombre.ToUpper());
                    else
                        sbCampos.Append(", ").Append(oCampo.Nombre.ToUpper());

                    //si la marca es de SQLite
                    if (this.cMarcaParametro == SQLiteUtiles.MARCADOR_PARAMETRO)
                        //utilizamos nros. en lugar de nombres de campos
                        if (sbValores.ToString().Equals(string.Empty))
                            sbValores.Append(this.cMarcaParametro + c.ToString());
                        else
                            sbValores.Append(", " + this.cMarcaParametro + c.ToString());            

                    else
                        //sino, la correspondiente al SGBD
                        if (sbValores.ToString().Equals(string.Empty))
                            sbValores.Append(this.cMarcaParametro + oCampo.Nombre.ToLower());
                        else
                            sbValores.Append(", " + this.cMarcaParametro + oCampo.Nombre.ToLower());

                    //incrementamos el contador
                    c++;

                }
            }

            //formateamos el comando y lo pasamos a donde corresponde
            Insert_sql = string.Format(__INSERT_SQL
                                        , __cNombreTabla
                                        , sbCampos.ToString()
                                        , sbValores.ToString()
                                        );

        }

        /// <summary>
        /// Establece la sentencia sql base de actualización de registros en la tabla administrada
        /// </summary>
        private void __SetUpdateSql()
        {
            //concatenamos la lista de campos NO autogenerados
            StringBuilder sbComando = new StringBuilder();
            int c = 1;

            //recorremos los campos de la tabla
            foreach (Campo oCampo in __dicCampos.Values)
            {
                //si no es generado por el propio SGBD
                if (!oCampo.AutoGenerado)
                {
                    //si la marca es SQLite
                    if (this.cMarcaParametro == SQLiteUtiles.MARCADOR_PARAMETRO)
                        //utilizamos nros. en lugar de nombres de campos
                        if (sbComando.ToString().Equals(string.Empty))
                            sbComando.Append(oCampo.Nombre.ToUpper()).Append(" = :").Append(c.ToString());
                        else
                            sbComando.Append(", ").Append(oCampo.Nombre.ToUpper()).Append(" = :").Append(c.ToString());

                    else
                        //sino, la correspondiente al SGBD
                        if (sbComando.ToString().Equals(string.Empty))
                            sbComando.Append(oCampo.Nombre.ToUpper()).Append(" = ").Append(this.cMarcaParametro + oCampo.Nombre.ToLower());
                        else
                            sbComando.Append(", ").Append(oCampo.Nombre.ToUpper()).Append(" = ").Append(this.cMarcaParametro + oCampo.Nombre.ToLower());

                    //incrementamos el contador
                    c++;

                }
            }

            //formateamos el comando y lo pasamos a donde corresponde
            Update_sql = string.Format(__UPDATE_SQL
                                        , __cNombreTabla
                                        , sbComando.ToString()
                                        , this.WhereSugerido
                                        );

        }

        /// <summary>
        /// Establece las condiciones de filtrado de registros para operaciones
        /// DML en base a la estructura especificada de la tabla
        /// </summary>
        private void __SetWhereSugerido()
        {
            //concatenamos la lista de condiciones de filtrado de registros a eliminar
            StringBuilder sbCondiciones = new StringBuilder(" 1 = 1 ");
            int c = 1;

            //recorremos los campos de la tabla
            foreach (Campo oCampo in __dicCampos.Values)
            {
                //si es PK
                if (oCampo.Identificador)
                {
                    //si la marca es SQLite
                    if (this.cMarcaParametro == SQLiteUtiles.MARCADOR_PARAMETRO)
                        //utilizamos nros. en lugar de nombres de campos
                        sbCondiciones.Append(" AND ").Append(oCampo.Nombre.ToUpper()).Append(" = ").Append(SQLiteUtiles.MARCADOR_PARAMETRO).Append(c.ToString());

                    else
                        //sino, la correspondiente al SGBD
                        sbCondiciones.Append(" AND ").Append(oCampo.Nombre.ToUpper()).Append(" = ").Append(this.cMarcaParametro).Append(oCampo.Nombre.ToLower());

                    //incrementamos el contador
                    c++;

                }
            }

            //si no hubo campos PK, entonces nos basamos en los campos AUTOGENERADOS como ultimo recurso
            if (c == 1)
            {
                //recorremos los campos de la tabla
                foreach (Campo oCampo in __dicCampos.Values)
                {
                    //si es autogenerado
                    if (oCampo.AutoGenerado)
                    {
                        //si la marca es SQLite
                        if (this.cMarcaParametro == SQLiteUtiles.MARCADOR_PARAMETRO)
                            //utilizamos nros. en lugar de nombres de campos
                            sbCondiciones.Append(" AND ").Append(oCampo.Nombre.ToUpper()).Append(" = ").Append(SQLiteUtiles.MARCADOR_PARAMETRO).Append(c.ToString());

                        else
                            //sino, la correspondiente al SGBD
                            sbCondiciones.Append(" AND ").Append(oCampo.Nombre.ToUpper()).Append(" = ").Append(this.cMarcaParametro).Append(oCampo.Nombre.ToLower());

                        //incrementamos el contador
                        c++;

                    }
                }
            }

            //formateamos el comando y lo pasamos a donde corresponde
            this.WhereSugerido = sbCondiciones.ToString();

        }

        /// <summary>
        /// Devuelve el diccionario de pares Parametro-Valor para las operaciones
        /// DML
        /// </summary>
        /// <param name="oOTDbase">Instancia de la cual se tomaran los valores de parametros</param>
        /// <param name="bIncluirNulos">Si se incluyen o no los pares con valores nulos</param>
        /// <returns>Diccionario de pares [NombrePropiedad, oValor]</returns>
        public Dictionary<string, object> ParametrosDML(OTDbase oOTDbase, bool bIncluirNulos)
        {
            var dicParametros = new Dictionary<string, object>();

            //recorremos los metodos de la clase actual
            foreach (var cCampo in __dicPropiedades.Keys)
            {
                //llamamos al metodo setter actual
                var oValor = __tTipoOTD.InvokeMember(__dicPropiedades[cCampo].Name
                                                    , BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.Public
                                                    , null, oOTDbase, new object[] { }
                                                    );

                //si el valor no es nulo
                if (oValor != null){
                
                    //si el valor pertenece a un identificador
                    if (__dicCampos[cCampo].Identificador)
                        //y su valor es cero o cadena vacía
                        if (oValor.ToString().Equals("0")
                                || oValor.ToString().Equals(string.Empty))
                            //lo saltamos
                            continue;

                    //si es de tipo DateTime
                    if (oValor.GetType() == typeof(DateTime))
                        //si es igual al valor minimo de este tipo, lo saltamos
                        if (((DateTime)oValor).Date == ADMbase.FECHA_MARCA_NULA) continue;

                    //si es de tipo long
                    if (oValor.GetType() == typeof(long))
                        //si es igual al valor minimo de este tipo, lo saltamos
                        if (((long)oValor) == long.MinValue) continue;

                    //si es de tipo int
                    if (oValor.GetType() == typeof(int))
                        //si es igual al valor minimo de este tipo, lo saltamos
                        if (((int)oValor) == int.MinValue) continue;

                    //si es de tipo double
                    if (oValor.GetType() == typeof(double))
                        //si es igual al valor minimo de este tipo, lo saltamos
                        if (((double)oValor) == double.MinValue) continue;

                    //si es de tipo decimal
                    if (oValor.GetType() == typeof(decimal))
                        //si es igual al valor minimo de este tipo, lo saltamos
                        if (((decimal)oValor) == decimal.MinValue) continue;

                    //si es de tipo float
                    if (oValor.GetType() == typeof(float))
                        //si es igual al valor minimo de este tipo, lo saltamos
                        if (((float)oValor) == float.MinValue) continue;
                
                    //recuperamos el valor de la propiedad y agregamos el par de parametros
                    dicParametros.Add(__dicCampos[cCampo].Nombre.ToLower(), oValor);

                }
                //sino, si el valor es nulo Y se requieren
                else if (bIncluirNulos)
                    //recuperamos el valor de la propiedad y agregamos el par de parametros
                    dicParametros.Add(__dicCampos[cCampo].Nombre.ToLower(), oValor);

            }

            //devolvemos el diccionario
            return dicParametros;
        }


        #endregion



        #region SOBREESCRITOS

        /// <summary>
        /// Devuelve la representacion de cadena de la clase
        /// </summary>
        /// <returns>Representacion de cadena de la clase</returns>
        public override string ToString()
        {
            return string.Format("<{0} - [{1}]> ", NOMBRE_CLASE, __cNombreTabla);
        }


        #endregion


    }
}
