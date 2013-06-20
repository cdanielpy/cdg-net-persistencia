using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System;
using CdgNetPersistenciaV3.ClasesBases;
using System.Reflection;
using CdgNetPersistenciaV3.Atributos;
using System.Text;

/**
    * Autor :  Cristian Daniel Garay Sosa
    * Fecha :  15/10/2012 (v3)
    * Comentarios:
    *          clase base de administracion de interaccion de la aplicacion 
    *          con los datos de una tabla y sus relaciones
    * 
    * **/

namespace CdgNetPersistenciaV3.ClasesBases
{
    /// <summary>
    /// Clase base de estructura de un administrador de Tabla
    /// </summary>
    public abstract class ADMbase
    {

        #region CAMPOS(Fields) DE LA CLASE

        public static string NOMBRE_CLASE = "ADMbase";

        /// <summary>
        /// Instancia de la clase de utileria de conexion a la base de datos
        /// </summary>
        protected ConectorBase _oConector;

        /// <summary>
        /// tipo de la clase extendida de OTDbase
        /// </summary>
        private Type __tTipoOTD;

        /// <summary>
        /// Para instancia modelo de generacion de codigos DML con retrasos
        /// </summary>
        private OTDbase __oInstanciaOTD;

        // cadenas para DML
        protected string _cListaCamposTabla;
        private string __cSelect_sql;
        private string __cInsert_sql;
        private string __cUpdate_sql;
        private string __cDelete_sql;
        private string __cWhereSugerido;

        // cadenas de sentencias bases
        private const string __SELECT_SQL = "SELECT {0} FROM {1}";
        private const string __INSERT_SQL = "INSERT INTO {0} ({1}) VALUES ({2})";
        private const string __UPDATE_SQL = "UPDATE {0} SET {1} WHERE {2}";
        private const string __DELETE_SQL = "DELETE FROM {0} WHERE {1}";

        /// <summary>
        /// Nombre de la tabla administrada
        /// </summary>
        private string __cNombreTabla;

        /// <summary>
        /// Marcador estandar de parametros de comandos SQL
        /// </summary>
        public const char MARCADOR_PARAMETRO_ESTANDAR = '?';

        /// <summary>
        /// Marcador de parametros SQL estandar
        /// </summary>
        public char cMarcaParametro = MARCADOR_PARAMETRO_ESTANDAR;

        public static DateTime FECHA_MARCA_NULA = DateTime.ParseExact("01/01/1753", "dd/MM/yyyy", null);

        #endregion



        #region CONSTRUCTORES

        /// <summary>
        /// Contructor de la clase
        /// </summary>
        /// <param name="oConectorBase">Instancia de utileria de interaccion con la base de datos</param>
        public ADMbase(ConectorBase oConectorBase)
        {
            _oConector = oConectorBase;
        }


        #endregion



        #region GETTERS Y SETTERS

        /// <summary>
        /// Instancia relacionada de la clase TBLbase
        /// </summary>
        protected void _Set_tabla<T>()
        {
            //si aun no esta instanciado
            if (__cNombreTabla == null)
            {
                //tomamos el tipo de la instancia
                __tTipoOTD = typeof(T);

                //recorremos sus atributos
                //http://msdn.microsoft.com/es-es/library/z919e8tw%28v=vs.80%29.aspx
                foreach (var oAtributo in __tTipoOTD.GetCustomAttributes(true))
                {
                    //si es del tipo Tabla
                    if (oAtributo is Tabla)
                    {
                        //tomamos el nombre de la tabla asignada
                        __cNombreTabla = (oAtributo as Tabla).Nombre;

                        //evaluamos el tipo de marcador de parametro segun el SGBD
                        switch ((oAtributo as Tabla).TipoSGBD)
                        {
                            case Tabla.SGBD.SQL_SERVER:
                                this.cMarcaParametro = SQLServerUtiles.MARCADOR_PARAMETRO;
                                break;
                            /*
                            case Tabla.SGBD.ORACLE:
                                this.cMarcaParametro = OracleUtiles.MARCADOR_PARAMETRO;
                                break;

                            case Tabla.SGBD.SQLITE:
                                this.cMarcaParametro = SQLiteUtiles.MARCADOR_PARAMETRO;
                                break;
                            */
                        }

                        //salimos del bucle
                        break;

                    }
                }

                //creamos una instancia
                __oInstanciaOTD = (Activator.CreateInstance<T>() as OTDbase);

                //invocamos a los metodos generadores de DML
                _SetListaCamposTabla();

                /**
                 * comentado para posponer la generacion de codigos DML hasta que sean solicitados
                 * **/

                /*
                _SetWhereSugerido<T>();
                _SetInsertSql<T>();
                _SetUpdateSql<T>();
                */


            }

        }

        /// <summary>
        /// Devuelveel nombre de la tabla administrada
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
                    __cSelect_sql = string.Format(__SELECT_SQL, _cListaCamposTabla, __cNombreTabla);
                return __cSelect_sql;
            }
            set
            { __cSelect_sql = value; }

        }

        /// <summary>
        /// Devuelve  o establece el comando de insercion de datos a la tabla
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
        /// Devuelve  o establece el comando de actualizacion de datos de la tabla
        /// </summary>
        public string Update_sql
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
        /// Devuelve o establece el comando de aliminacion de datos de la tabla
        /// </summary>
        public string Delete_sql
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
        public string WhereSugerido
        {
            get
            {
                if (__cWhereSugerido == null || __cWhereSugerido == string.Empty)
                    __SetWhereSugerido();
                return __cWhereSugerido;
            }
            set { __cWhereSugerido = value; }
        }



        #endregion



        #region METODOS DML

        /// <summary>
        /// Inserta un nuevo registro en la tabla a partir de la instancia
        /// </summary>
        /// <param name="oOTDbase">Instancia de la clase que representa a un registro</param>
        /// <returns>Lista [entero, resultado]</returns>
        public abstract List<object> lAgregar(OTDbase oOTDbase);

        /// <summary>
        /// Inserta un nuevo registro en la tabla a partir de la instancia parametro
        /// </summary>
        /// <typeparam name="T">Clase extendida de OTDbase de la cual se obtienen los datos
        /// del registro a insertar</typeparam>
        /// <param name="oOTDbase">Instancia de la clase que contiene los datos a insertar</param>
        /// <returns>Lista [entero, resultado]</returns>
        public virtual List<object> lAgregar<T>(OTDbase oOTDbase)
        {

            string NOMBRE_METODO = NOMBRE_CLASE + ".lAgregar()";

            // lista con el resultado del metodo 
            List<object> lResultado = new List<object>() { 0, NOMBRE_METODO + " No Ejecutado!" };

            try
            {
                //si el tipo es del mismo que se esta administrando
                if (__tTipoOTD == typeof(T))
                {
                    //ejecutamos la sentencia de actualizacion
                    lResultado = _oConector.lEjecutar_sentencia(Insert_sql, oOTDbase.ParametrosDML(true));

                    //si se inserto correctamente, recuperamos la misma instancia desde la base de datos
                    if ((int)lResultado[0] == 1) return this.lGet_elemento(oOTDbase);

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
                lResultado[0] = -1;
                lResultado[1] = NOMBRE_METODO + " Error: " + ex.Message;
            }

            //devolvemos el resultado del metodo
            return lResultado;

        }

        /// <summary>
        /// Actualiza el registro en la tabla a partir de la instancia parametro
        /// </summary>
        /// <param name="oOTDbase">Instancia de la clase que representa a un registro</param>
        /// <returns>Lista [entero, resultado]</returns>
        public abstract List<object> lActualizar(OTDbase oOTDbase);

        /// <summary>
        /// Actualiza el registro en la tabla a partir de la instancia parametro
        /// </summary>
        /// <typeparam name="T">Clase extendida de OTDbase de la cual se obtienen los datos
        /// del registro a actualizar y sus nuevos valores</typeparam>
        /// <param name="oOTDbase">Instancia de la clase que representa a un registro</param>
        /// <returns>Lista [entero, resultado]</returns>
        public virtual List<object> lActualizar<T>(OTDbase oOTDbase)
        {
            string NOMBRE_METODO = NOMBRE_CLASE + ".lActualizar()";

            // lista con el resultado del metodo 
            List<object> lResultado = new List<object>() { 0, NOMBRE_METODO + " No Ejecutado!" };

            try
            {
                //si el tipo es del mismo que se esta administrando
                if (__tTipoOTD == typeof(T))
                {
                    //ejecutamos la sentencia de actualizacion
                    lResultado = _oConector.lEjecutar_sentencia(Update_sql, oOTDbase.ParametrosDML(true));

                    //si se inserto correctamente, recuperamos la misma instancia desde la base de datos
                    if ((int)lResultado[0] == 1) return this.lGet_elemento(oOTDbase);

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
                lResultado[0] = -1;
                lResultado[1] = NOMBRE_METODO + " Error: " + ex.Message;
            }

            //devolvemos el resultado del metodo
            return lResultado;

        }

        /// <summary>
        /// Elimina el registro de la tabla representado por la instancia parametro
        /// </summary>
        /// <param name="oOTDbase">Instancia de la clase que representa a un registro</param>
        /// <returns>Lista [entero, resultado]</returns>
        public abstract List<object> lEliminar(OTDbase oOTDbase);

        /// <summary>
        /// Elimina el registro de la tabla representado por la instancia parametro
        /// </summary>
        /// <typeparam name="T">Clase extendida de OTDbase de la cual se obtienen los datos
        /// del registro a eliminar</typeparam>
        /// <param name="oOTDbase">Instancia de la clase que representa al registro a eliminar</param>
        /// <returns>Lista [entero, resultado]</returns>
        public virtual List<object> lEliminar<T>(OTDbase oOTDbase)
        {
            string NOMBRE_METODO = NOMBRE_CLASE + ".lEliminar()";

            // lista con el resultado del metodo 
            List<object> lResultado = new List<object>() { 0, NOMBRE_METODO + " No Ejecutado!" };

            try
            {
                //si el tipo es del mismo que se esta administrando
                if (__tTipoOTD == typeof(T))
                {
                    //ejecutamos la sentencia de actualizacion
                    return _oConector.lEjecutar_sentencia(Delete_sql, oOTDbase.ParametrosDML(true));
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
                lResultado[0] = -1;
                lResultado[1] = NOMBRE_METODO + " Error: " + ex.Message;
            }

            //devolvemos el resultado del metodo
            return lResultado;
        }

        /// <summary>
        /// Obtiene una unica instancia que coincide con el Id buscado
        /// </summary>
        /// <param name="oOTDbase">Instancia de la clase que representa a un registro</param>
        /// <returns>Lista [entero, resultado]</returns>
        public abstract List<object> lGet_elemento(OTDbase oOTDbase);

        /// <summary>
        /// Obtiene una unica instancia que coincide con el Id buscado
        /// </summary>
        /// <typeparam name="T">Clase extendida de OTDbase a la cual se transfieren los datos
        /// del registro</typeparam>
        /// <param name="oOTDbase">Instancia de la clase que representa a un registro</param>
        /// <returns>Lista [entero, resultado]</returns>
        public virtual List<object> lGet_elemento<T>(OTDbase oOTDbuscado)
        {
            string NOMBRE_METODO = NOMBRE_CLASE + ".lGet_elemento()";

            //lista para  resultado del metodo 
            List<object> lResultado = new List<object>() { 0, NOMBRE_METODO + " No Ejecutado!" };

            try
            {
                //si el tipo es del mismo que se esta administrando
                if (__tTipoOTD == typeof(T))
                {
                    // variable para el filtrado
                    string cFiltroWhere = string.Empty;

                    //tomamos el diccionario de propiedades de la instancia
                    var dicPropiedades = oOTDbuscado.Get_dic_propiedades();

                    //recorremos los campos de la tabla referenciada
                    foreach (string cNombreCampo in dicPropiedades.Keys)
                    {
                        //llamamos al metodo getter de la propiedad actual
                        var oValor = __tTipoOTD.InvokeMember(dicPropiedades[cNombreCampo].Name
                                                            , BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.Public
                                                            , null, oOTDbuscado
                                                            , new object[] { }
                                                            );

                        //si no es nulo
                        if (oValor != null)
                        {
                            //si es un identificador y no esta vacio
                            if (oOTDbuscado.Get_dic_campos()[cNombreCampo].Identificador
                                    && oValor.ToString() != "0"
                                    && oValor.ToString() != string.Empty
                                )
                            {
                                //lo agregamos a la condicion de filtrado
                                if (cFiltroWhere == string.Empty)
                                    cFiltroWhere = string.Format(" WHERE {0} = {1}{2} "
                                                                , cNombreCampo
                                                                , this.cMarcaParametro
                                                                , oOTDbuscado.Get_dic_campos()[cNombreCampo].Nombre.ToLower()
                                                                );
                                else
                                    //sino, es un AND
                                    cFiltroWhere += string.Format(" AND {0} = {1}{2} "
                                                                , cNombreCampo
                                                                , this.cMarcaParametro
                                                                , oOTDbuscado.Get_dic_campos()[cNombreCampo].Nombre.ToLower()
                                                                );
                            }
                            else if (!oOTDbuscado.Get_dic_campos()[cNombreCampo].Identificador)
                            {
                                //si es de tipo DateTime
                                if (oValor.GetType() == typeof(DateTime))
                                    //si es igual al valor minimo de este tipo, lo saltamos
                                    if (((DateTime)oValor).Date == ADMbase.FECHA_MARCA_NULA) goto salto;

                                //si es de tipo long
                                if (oValor.GetType() == typeof(long))
                                    //si es igual al valor minimo de este tipo, lo saltamos
                                    if (((long)oValor) == long.MinValue) goto salto;

                                //si es de tipo int
                                if (oValor.GetType() == typeof(int))
                                    //si es igual al valor minimo de este tipo, lo saltamos
                                    if (((int)oValor) == int.MinValue) goto salto;

                                //si es de tipo double
                                if (oValor.GetType() == typeof(double))
                                    //si es igual al valor minimo de este tipo, lo saltamos
                                    if (((double)oValor) == double.MinValue) goto salto;

                                //si es de tipo decimal
                                if (oValor.GetType() == typeof(decimal))
                                    //si es igual al valor minimo de este tipo, lo saltamos
                                    if (((decimal)oValor) == decimal.MinValue) goto salto;

                                //si es de tipo float
                                if (oValor.GetType() == typeof(float))
                                    //si es igual al valor minimo de este tipo, lo saltamos
                                    if (((float)oValor) == float.MinValue) goto salto;

                                //lo agregamos a la condicion de filtrado
                                if (cFiltroWhere == string.Empty)
                                    cFiltroWhere = string.Format(" WHERE {0} = {1}{2} "
                                                                , cNombreCampo
                                                                , this.cMarcaParametro
                                                                , oOTDbuscado.Get_dic_campos()[cNombreCampo].Nombre.ToLower()
                                                                );
                                else
                                    //sino, es un AND
                                    cFiltroWhere += string.Format(" AND {0} = {1}{2} "
                                                                , cNombreCampo
                                                                , this.cMarcaParametro
                                                                , oOTDbuscado.Get_dic_campos()[cNombreCampo].Nombre.ToLower()
                                                                );
                            }

                        //etiqueta para saltarse la adicion de elementos a lacondicion de filtrado
                        salto:
                            var i = 0;
                        }
                    }

                    //invocamos al metodo de recuperacion de registros desde la tabla
                    lResultado = this.lGet_elementos(cFiltroWhere, oOTDbuscado.ParametrosDML(false));

                    //si se ejecuto correctamente, devolvemos solo el primer elemento de la lista de instancias devueltas
                    if ((int)lResultado[0] == 1) return new List<object>() { 1, (lResultado[1] as List<T>)[0] };

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
                lResultado[0] = -1;
                lResultado[1] = NOMBRE_METODO + " Error: " + ex.Message;
            }

            //devolvemos el resultado del metodo
            return lResultado;

        }

        /// <summary>
        /// Obtiene una lista de instancias que satisfacen 
        /// la condicion de filtrado desde la base de datos
        /// </summary>
        /// <param name="cFiltroWhere">Condicion de filtrado de registros</param>
        /// <param name="dicParametros">Diccionario de claves y valores de parametros de la sentencia</param>
        /// <returns>Lista de resultado [entero, object]</returns>
        public abstract List<object> lGet_elementos(string cFiltroWhere, Dictionary<string, object> dicParametros);

        /// <summary>
        /// Obtiene una lista de instancias que satisfacen 
        /// la condicion de filtrado desde la base de datos
        /// </summary>
        /// <typeparam name="T">Clase extendida de OTDbase a la cual se transfieren los datos 
        /// de registros</typeparam>
        /// <param name="cFiltroWhere">Condicion de filtrado de registros</param>
        /// <param name="dicParametros">Diccionario de claves y valores de parametros de la sentencia</param>
        /// <returns></returns>
        public virtual List<object> lGet_elementos<T>(string cFiltroWhere, Dictionary<string, object> dicParametros)
        {
            string NOMBRE_METODO = NOMBRE_CLASE + ".lGet_elementos()";

            //una lista para le resultado del metodo 
            List<object> lResultado = new List<object>() { 0, NOMBRE_METODO + " No Ejecutado!" };

            try
            {
                //invocamos al metodo de ejecucion de la consulta
                lResultado = _oConector.lEjecutar_consulta(Select_sql + cFiltroWhere, dicParametros);

                //si se ejecuto correctamente,invocamos al metodo de conversion a coleccion de OTDs y establecemos el resultado
                if ((int)lResultado[0] == 1) return new List<object>() { 1, _lSet_registros<T>((lResultado[1] as DataTable).Rows) };

            }
            catch (Exception ex)
            {
                //en caso de error, establecemos el resultado como corresponde
                lResultado[0] = -1;
                lResultado[1] = NOMBRE_METODO + " Error: " + ex.Message;
            }

            //devolvemos el resultado del metodo
            return lResultado;

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
        public virtual List<object> lRecrear_tabla(string cStringDrop, string cStringCreate)
        {

            string NOMBRE_METODO = NOMBRE_CLASE + ".lRecrear_tabla()";

            //lista de resultado
            List<object> lResultado = new List<object>();

            try
            {
                //si se recibe la cadena de eliminacion de la tabla
                if (cStringDrop != null
                    && cStringDrop != string.Empty)
                    //la ejecutamos
                    lResultado = _oConector.lEjecutar_sentencia(cStringDrop, new Dictionary<string, object>());
                else
                    //sino, la sentencia por defecto
                    lResultado = _oConector.lEjecutar_sentencia(_oTabla.Get_drop_tabla, new Dictionary<string, object>());

                //si se ejecuto correctamente
                if ((int)lResultado[0] == 1)
                {
                    //si se recibe la cadena de creacion de la tabla
                    if (cStringCreate != null
                        && cStringCreate != string.Empty)
                        //la ejecutamos
                        lResultado = _oConector.lEjecutar_sentencia(cStringCreate, new Dictionary<string, object>());
                    else
                        //sino, la sentencia por defecto
                        lResultado = _oConector.lEjecutar_sentencia(_oTabla.Get_create_tabla, new Dictionary<string, object>());
                }

            }
            catch (Exception ex)
            {
                //en caso de error
                lResultado[0] = -1;
                lResultado[1] = NOMBRE_METODO + " Error: " + ex.Message;
            }

            //devolvemos el resultado del metodo
            return lResultado;

        }
        */


        #endregion



        #region METODOS DE PASO OTD -> DATOS - DATOS -> OTD

        /// <summary>
        /// Ejecuta la instanciacion y asignacion de los OTDs 
        /// </summary>
        /// <typeparam name="T">Clase de OTDbase extendida a los que seran 'convertidas' las filas</typeparam>
        // <param name="lInstancias">Lista de OTDs a cargar con las instancias generadas</param>
        /// <param name="aFilasParam">Arreglo de filas de un DataTable</param>
        /// <see cref="http://www.csharp-examples.net/reflection-examples/" />
        protected List<T> _lSet_registros<T>(DataRowCollection aFilasParam)
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

                    //tomamos el diccionario de propiedades de la instancia
                    var dicPropiedades = (oInstanciaOTD as OTDbase).Get_dic_propiedades();

                    //recorremos los campos de la tabla referenciada
                    foreach (string cNombreCampo in dicPropiedades.Keys)
                    {
                        //si no es un valor nulo
                        if (dr[cNombreCampo].GetType() != typeof(DBNull))
                            //llamamos al metodo setter actual
                            __tTipoOTD.InvokeMember(dicPropiedades[cNombreCampo].Name
                                                    , BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.Public
                                                    , null, oInstanciaOTD
                                                    , new object[] { dr[cNombreCampo] }
                                                    );
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

        /**
         * GENERADORES PARA SOBREESCRIBIR LOS ACTUALES DESDE LA CLASE EXTENDIDA
         * **/

        /// <summary>
        ///  Devuelve una cadena con los nombres de los campos con alias de la incluido
        /// TABLA.CAMPO0, TABLA.CAMPO1, TABLA.CAMPO2, ...
        /// </summary>
        /// <typeparam name="T">Clase de OTDbase extendida a los que representan a las filas de la tabla</typeparam>
        protected void _SetListaCamposTabla()
        {
            //la escribimos
            StringBuilder sbLista = new StringBuilder();

            //recorremos los campos
            foreach (Campo oCampo in __oInstanciaOTD.Get_dic_campos().Values)
            {
                if (sbLista.Length.Equals(0)) sbLista.Append(__cNombreTabla).Append(".").Append(oCampo.Nombre);
                else sbLista.Append(", ").Append(__cNombreTabla).Append(".").Append(oCampo.Nombre);
            }

            //tomamos la cadena
            this._cListaCamposTabla = sbLista.ToString();

        }

        /// <summary>
        /// Establece la sentencia base de insercion de registros en la tabla administrada
        /// </summary>
        /// <typeparam name="T">Clase de OTDbase extendida a los que representan a las filas de la tabla</typeparam>
        protected void _SetInsertSql<T>()
        {
            //creamos una instancia
            OTDbase oInstanciaOTD = (Activator.CreateInstance<T>() as OTDbase);

            //concatenamos la lista de campos NO autogenerados
            StringBuilder sbCampos = new StringBuilder();
            StringBuilder sbValores = new StringBuilder();
            int c = 1;

            //recorremos los campos de la tabla
            foreach (Campo oCampo in oInstanciaOTD.Get_dic_campos().Values)
            {
                //si no es generado por el propio SGBD
                if (!oCampo.AutoGenerado)
                {
                    if (sbCampos.ToString().Equals(string.Empty)) sbCampos.Append(oCampo.Nombre.ToUpper());
                    else sbCampos.Append(", ").Append(oCampo.Nombre.ToUpper());

                    //si la marca es SQLite
                    if (this.cMarcaParametro == SQLiteUtiles.MARCADOR_PARAMETRO)
                        //utilizamos nros. en lugar de nombres de campos
                        if (sbValores.ToString().Equals(string.Empty))
                            sbValores.Append(":" + c.ToString());
                        else
                            sbValores.Append(", :" + c.ToString());

                    //sino, si la marca es estandar
                    else if (this.cMarcaParametro == MARCADOR_PARAMETRO_ESTANDAR)
                        //utilizamos solo el marcador sin los campos
                        if (sbValores.ToString().Equals(string.Empty))
                            sbValores.Append(this.cMarcaParametro);
                        else
                            sbValores.Append(", " + this.cMarcaParametro);

                    else
                        //sino, la correspondiente al SGBD
                        if (sbValores.ToString().Equals(string.Empty))
                            sbValores.Append(this.cMarcaParametro + oCampo.Nombre.ToLower());
                        else
                            sbValores.Append(", " + this.cMarcaParametro + oCampo.Nombre.ToLower());


                    //incrementamos el contador
                    c += 1;
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
        /// Establece la sentencia base de actualizacion de registros en la tabla administrada
        /// </summary>
        /// <typeparam name="T">Clase de OTDbase extendida a los que representan a las filas de la tabla</typeparam>
        protected void _SetUpdateSql<T>()
        {
            //creamos una instancia
            OTDbase oInstanciaOTD = (Activator.CreateInstance<T>() as OTDbase);

            //concatenamos la lista de campos NO autogenerados
            StringBuilder sbComando = new StringBuilder();
            int c = 1;

            //recorremos los campos de la tabla
            foreach (Campo oCampo in oInstanciaOTD.Get_dic_campos().Values)
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

                    //sino, si la marca es estandar
                    else if (this.cMarcaParametro == MARCADOR_PARAMETRO_ESTANDAR)
                        //utilizamos solo el marcador sin los campos
                        if (sbComando.ToString().Equals(string.Empty))
                            sbComando.Append(oCampo.Nombre.ToUpper()).Append(" = ").Append(this.cMarcaParametro);
                        else
                            sbComando.Append(", ").Append(oCampo.Nombre.ToUpper()).Append(" = ").Append(this.cMarcaParametro);

                    else
                        //sino, la correspondiente al SGBD
                        if (sbComando.ToString().Equals(string.Empty))
                            sbComando.Append(oCampo.Nombre.ToUpper()).Append(" = ").Append(this.cMarcaParametro + oCampo.Nombre.ToLower());
                        else
                            sbComando.Append(", ").Append(oCampo.Nombre.ToUpper()).Append(" = ").Append(this.cMarcaParametro + oCampo.Nombre.ToLower());

                    //incrementamos el contador
                    c += 1;

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
        /// <typeparam name="T"></typeparam>
        protected void _SetWhereSugerido<T>()
        {
            //creamos una instancia
            OTDbase oInstanciaOTD = (Activator.CreateInstance<T>() as OTDbase);

            //concatenamos la lista de condiciones de filtrado de registros a eliminar
            StringBuilder sbCondiciones = new StringBuilder();
            int c = 1;

            //recorremos los campos de la tabla
            foreach (Campo oCampo in oInstanciaOTD.Get_dic_campos().Values)
            {
                //si es PK
                if (oCampo.Identificador)
                {
                    //si la marca es SQLite
                    if (this.cMarcaParametro == SQLiteUtiles.MARCADOR_PARAMETRO)
                        //utilizamos nros. en lugar de nombres de campos
                        if (sbCondiciones.ToString().Equals(string.Empty))
                            sbCondiciones.Append(oCampo.Nombre.ToUpper()).Append(" = :").Append(c.ToString());
                        else
                            sbCondiciones.Append(", ").Append(oCampo.Nombre.ToUpper()).Append(" = :").Append(c.ToString());

                    //sino, si la marca es estandar
                    else if (this.cMarcaParametro == MARCADOR_PARAMETRO_ESTANDAR)
                        //utilizamos solo el marcador sin los campos
                        if (sbCondiciones.ToString().Equals(string.Empty))
                            sbCondiciones.Append(oCampo.Nombre.ToUpper()).Append(" = ").Append(this.cMarcaParametro);
                        else
                            sbCondiciones.Append(", ").Append(oCampo.Nombre.ToUpper()).Append(" = ").Append(this.cMarcaParametro);

                    else
                        //sino, la correspondiente al SGBD
                        if (sbCondiciones.ToString().Equals(string.Empty))
                            //sbCondiciones.Append(oCampo.Nombre.ToUpper()).Append(" = ").Append(this.cMarcaParametro + oInstanciaOTD.Get_dic_propiedades()[oCampo.Nombre].Name.ToLower());
                            sbCondiciones.Append(oCampo.Nombre.ToUpper()).Append(" = ").Append(this.cMarcaParametro + oCampo.Nombre.ToLower());
                        else
                            //sbCondiciones.Append(", ").Append(oCampo.Nombre.ToUpper()).Append(" = ").Append(this.cMarcaParametro + oInstanciaOTD.Get_dic_propiedades()[oCampo.Nombre].Name.ToLower());
                            sbCondiciones.Append(", ").Append(oCampo.Nombre.ToUpper()).Append(" = ").Append(this.cMarcaParametro + oCampo.Nombre.ToLower());

                    //incrementamos el contador
                    c += 1;

                }
            }

            //si no hubo campos PK, entonces nos basamos en los campos AUTOGENERADOS como ultimo recurso
            if (c == 1)
            {
                //recorremos los campos de la tabla
                foreach (Campo oCampo in oInstanciaOTD.Get_dic_campos().Values)
                {
                    //si es autogenerado
                    if (oCampo.AutoGenerado)
                    {
                        //si la marca es SQLite
                        if (this.cMarcaParametro == SQLiteUtiles.MARCADOR_PARAMETRO)
                            //utilizamos nros. en lugar de nombres de campos
                            if (sbCondiciones.ToString().Equals(string.Empty))
                                sbCondiciones.Append(oCampo.Nombre.ToUpper()).Append(" = :").Append(c.ToString());
                            else
                                sbCondiciones.Append(", ").Append(oCampo.Nombre.ToUpper()).Append(" = :").Append(c.ToString());

                        //sino, si la marca es estandar
                        else if (this.cMarcaParametro == MARCADOR_PARAMETRO_ESTANDAR)
                            //utilizamos solo el marcador sin los campos
                            if (sbCondiciones.ToString().Equals(string.Empty))
                                sbCondiciones.Append(oCampo.Nombre.ToUpper()).Append(" = ").Append(this.cMarcaParametro);
                            else
                                sbCondiciones.Append(", ").Append(oCampo.Nombre.ToUpper()).Append(" = ").Append(this.cMarcaParametro);

                        else
                            //sino, la correspondiente al SGBD
                            if (sbCondiciones.ToString().Equals(string.Empty))
                                sbCondiciones.Append(oCampo.Nombre.ToUpper()).Append(" = ").Append(this.cMarcaParametro + oCampo.Nombre.ToLower());
                            else
                                sbCondiciones.Append(", ").Append(oCampo.Nombre.ToUpper()).Append(" = ").Append(this.cMarcaParametro + oCampo.Nombre.ToLower());

                        //incrementamos el contador
                        c += 1;

                    }
                }
            }

            //formateamos el comando y lo pasamos a donde corresponde
            this.WhereSugerido = sbCondiciones.ToString();

        }

        /**
         * GENERADORES SOBRE PEDIDO INTERNOS DESDE LOS ACCESORES
         * **/

        /// <summary>
        /// Establece la sentencia base de insercion de registros en la tabla administrada
        /// </summary>
        private void __SetInsertSql()
        {
            //concatenamos la lista de campos NO autogenerados
            StringBuilder sbCampos = new StringBuilder();
            StringBuilder sbValores = new StringBuilder();
            int c = 1;

            //recorremos los campos de la tabla
            foreach (Campo oCampo in __oInstanciaOTD.Get_dic_campos().Values)
            {
                //si no es generado por el propio SGBD
                if (!oCampo.AutoGenerado)
                {
                    if (sbCampos.ToString().Equals(string.Empty)) sbCampos.Append(oCampo.Nombre.ToUpper());
                    else sbCampos.Append(", ").Append(oCampo.Nombre.ToUpper());

                    //si la marca es SQLite
                    if (this.cMarcaParametro == SQLiteUtiles.MARCADOR_PARAMETRO)
                        //utilizamos nros. en lugar de nombres de campos
                        if (sbValores.ToString().Equals(string.Empty))
                            sbValores.Append(":" + c.ToString());
                        else
                            sbValores.Append(", :" + c.ToString());

                    //sino, si la marca es estandar
                    else if (this.cMarcaParametro == MARCADOR_PARAMETRO_ESTANDAR)
                        //utilizamos solo el marcador sin los campos
                        if (sbValores.ToString().Equals(string.Empty))
                            sbValores.Append(this.cMarcaParametro);
                        else
                            sbValores.Append(", " + this.cMarcaParametro);

                    else
                        //sino, la correspondiente al SGBD
                        if (sbValores.ToString().Equals(string.Empty))
                            sbValores.Append(this.cMarcaParametro + oCampo.Nombre.ToLower());
                        else
                            sbValores.Append(", " + this.cMarcaParametro + oCampo.Nombre.ToLower());


                    //incrementamos el contador
                    c += 1;
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
        /// Establece la sentencia base de actualizacion de registros en la tabla administrada
        /// </summary>
        private void __SetUpdateSql()
        {
            //concatenamos la lista de campos NO autogenerados
            StringBuilder sbComando = new StringBuilder();
            int c = 1;

            //recorremos los campos de la tabla
            foreach (Campo oCampo in __oInstanciaOTD.Get_dic_campos().Values)
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

                    //sino, si la marca es estandar
                    else if (this.cMarcaParametro == MARCADOR_PARAMETRO_ESTANDAR)
                        //utilizamos solo el marcador sin los campos
                        if (sbComando.ToString().Equals(string.Empty))
                            sbComando.Append(oCampo.Nombre.ToUpper()).Append(" = ").Append(this.cMarcaParametro);
                        else
                            sbComando.Append(", ").Append(oCampo.Nombre.ToUpper()).Append(" = ").Append(this.cMarcaParametro);

                    else
                        //sino, la correspondiente al SGBD
                        if (sbComando.ToString().Equals(string.Empty))
                            sbComando.Append(oCampo.Nombre.ToUpper()).Append(" = ").Append(this.cMarcaParametro + oCampo.Nombre.ToLower());
                        else
                            sbComando.Append(", ").Append(oCampo.Nombre.ToUpper()).Append(" = ").Append(this.cMarcaParametro + oCampo.Nombre.ToLower());

                    //incrementamos el contador
                    c += 1;

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
            StringBuilder sbCondiciones = new StringBuilder();
            int c = 1;

            //recorremos los campos de la tabla
            foreach (Campo oCampo in __oInstanciaOTD.Get_dic_campos().Values)
            {
                //si es PK
                if (oCampo.Identificador)
                {
                    //si la marca es SQLite
                    if (this.cMarcaParametro == SQLiteUtiles.MARCADOR_PARAMETRO)
                        //utilizamos nros. en lugar de nombres de campos
                        if (sbCondiciones.ToString().Equals(string.Empty))
                            sbCondiciones.Append(oCampo.Nombre.ToUpper()).Append(" = :").Append(c.ToString());
                        else
                            sbCondiciones.Append(", ").Append(oCampo.Nombre.ToUpper()).Append(" = :").Append(c.ToString());

                    //sino, si la marca es estandar
                    else if (this.cMarcaParametro == MARCADOR_PARAMETRO_ESTANDAR)
                        //utilizamos solo el marcador sin los campos
                        if (sbCondiciones.ToString().Equals(string.Empty))
                            sbCondiciones.Append(oCampo.Nombre.ToUpper()).Append(" = ").Append(this.cMarcaParametro);
                        else
                            sbCondiciones.Append(", ").Append(oCampo.Nombre.ToUpper()).Append(" = ").Append(this.cMarcaParametro);

                    else
                        //sino, la correspondiente al SGBD
                        if (sbCondiciones.ToString().Equals(string.Empty))
                            //sbCondiciones.Append(oCampo.Nombre.ToUpper()).Append(" = ").Append(this.cMarcaParametro + oInstanciaOTD.Get_dic_propiedades()[oCampo.Nombre].Name.ToLower());
                            sbCondiciones.Append(oCampo.Nombre.ToUpper()).Append(" = ").Append(this.cMarcaParametro + oCampo.Nombre.ToLower());
                        else
                            //sbCondiciones.Append(", ").Append(oCampo.Nombre.ToUpper()).Append(" = ").Append(this.cMarcaParametro + oInstanciaOTD.Get_dic_propiedades()[oCampo.Nombre].Name.ToLower());
                            sbCondiciones.Append(", ").Append(oCampo.Nombre.ToUpper()).Append(" = ").Append(this.cMarcaParametro + oCampo.Nombre.ToLower());

                    //incrementamos el contador
                    c += 1;

                }
            }

            //si no hubo campos PK, entonces nos basamos en los campos AUTOGENERADOS como ultimo recurso
            if (c == 1)
            {
                //recorremos los campos de la tabla
                foreach (Campo oCampo in __oInstanciaOTD.Get_dic_campos().Values)
                {
                    //si es autogenerado
                    if (oCampo.AutoGenerado)
                    {
                        //si la marca es SQLite
                        if (this.cMarcaParametro == SQLiteUtiles.MARCADOR_PARAMETRO)
                            //utilizamos nros. en lugar de nombres de campos
                            if (sbCondiciones.ToString().Equals(string.Empty))
                                sbCondiciones.Append(oCampo.Nombre.ToUpper()).Append(" = :").Append(c.ToString());
                            else
                                sbCondiciones.Append(", ").Append(oCampo.Nombre.ToUpper()).Append(" = :").Append(c.ToString());

                        //sino, si la marca es estandar
                        else if (this.cMarcaParametro == MARCADOR_PARAMETRO_ESTANDAR)
                            //utilizamos solo el marcador sin los campos
                            if (sbCondiciones.ToString().Equals(string.Empty))
                                sbCondiciones.Append(oCampo.Nombre.ToUpper()).Append(" = ").Append(this.cMarcaParametro);
                            else
                                sbCondiciones.Append(", ").Append(oCampo.Nombre.ToUpper()).Append(" = ").Append(this.cMarcaParametro);

                        else
                            //sino, la correspondiente al SGBD
                            if (sbCondiciones.ToString().Equals(string.Empty))
                                sbCondiciones.Append(oCampo.Nombre.ToUpper()).Append(" = ").Append(this.cMarcaParametro + oCampo.Nombre.ToLower());
                            else
                                sbCondiciones.Append(", ").Append(oCampo.Nombre.ToUpper()).Append(" = ").Append(this.cMarcaParametro + oCampo.Nombre.ToLower());

                        //incrementamos el contador
                        c += 1;

                    }
                }
            }

            //formateamos el comando y lo pasamos a donde corresponde
            this.WhereSugerido = sbCondiciones.ToString();

        }


        #endregion



        #region SOBREESCRITOS

        /// <summary>
        /// Devuelve la representacion de la clase
        /// </summary>
        /// <returns>Representacion de cadena de la clase</returns>
        public override string ToString()
        {
            return string.Format("<{0} - [{1}]> ", NOMBRE_CLASE, __cNombreTabla);
        }


        #endregion


    }
}
