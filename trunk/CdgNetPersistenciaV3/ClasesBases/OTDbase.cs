using System;
using System.Collections.Generic;

using System.Text;
using System.Data;
using System.Reflection;
using CdgNetPersistenciaV3.Atributos;

  /**
    * Autor :  Cristian Daniel Garay Sosa
    * Fecha :  23/10/2012 (v3)
    * Comentarios:
    *          representa a una fila de una tabla pudiendo extenderse segun se necesite
    * 
    * 
    * **/

namespace CdgNetPersistenciaV3.ClasesBases
{
    /// <summary>
    /// Clase base de Objetos Transportadores de Datos
    /// </summary>
    public abstract class OTDbase
    {

        #region CAMPOS(Fields)

        /// <summary>
        /// Almacena el nombre de la clase
        /// </summary>
        public static string NOMBRE_CLASE = "OTDbase";

        /// <summary>
        /// Almacena el valor del identificador de la instancia
        /// Normalmente debería ser el valor del campo PK de la tabla
        /// </summary>
        protected long _nId;

        /// <summary>
        /// Almacena la descripción de la instancia
        /// </summary>
        protected string _cDescripcion;

        /// <summary>
        /// Dicionario para relacionar los campos de la 
        /// instancia a los Campos de la tabla
        /// </summary>
        Dictionary<string, Campo> __dicCampos;

        /// <summary>
        /// Diccionario para relacionar campos de Tabla y Propiedades
        /// referenciando a los mismos por medio de Campos
        /// </summary>
        Dictionary<string, PropertyInfo> __dicPropiedades;


        #endregion



        #region CONSTRUCTORES

        /// <summary>
        /// Constructor por defecto de la clase
        /// </summary>
        public OTDbase()
        {
        }

        /// <summary>
        /// Constructor de la clase
        /// </summary>
        /// <param name="nIdParam">Identificador</param>
        /// <param name="cDescripcionParam">Descripcion</param>
        public OTDbase(long nIdParam, string cDescripcionParam)
        {
            _nId = nIdParam;
            _cDescripcion = cDescripcionParam;    
        }

        #endregion



        #region GETTERS Y SETTERS


        public virtual long Id {get; set; }

        public virtual string Descripcion { get; set; }

        #endregion



        #region METODOS

        /// <summary>
        /// Crea y devuelve un clon de la instancia actual
        /// </summary>
        /// <typeparam name="T">Clase de la instancia a crear</typeparam>
        /// <returns>Instancia Clon</returns>
        public T Get_clon<T>()
        {
            //creamos una instancia del tipo
            Type tTipoOTD = typeof(T);

            //creamos una instancia del OTD extendido
            object oInstanciaOTD = Activator.CreateInstance<T>();

            //tomamos el diccionario de propiedades de la instancia
            var dicPropiedades = (oInstanciaOTD as OTDbase).Get_dic_propiedades();

            //tomamos los campos de la clase
            PropertyInfo[] aPropiedades = tTipoOTD.GetProperties(BindingFlags.GetProperty
                                                                    | BindingFlags.Instance
                                                                    | BindingFlags.Public
                                                                    );

            //recorremos los campos de la tabla referenciada
            //foreach (string cNombreCampo in dicPropiedades.Keys)
            foreach (PropertyInfo oInfo in aPropiedades)
            {
                //llamamos al metodo getter actual
                object oValor = tTipoOTD.InvokeMember(oInfo.Name
                                                        , BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.Public
                                                        , null, this
                                                        , new object[] { }
                                                        );

                //si no es un valor nulo
                if (oValor != null)
                    //llamamos al metodo setter actual
                    tTipoOTD.InvokeMember(oInfo.Name
                                            , BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.Public
                                            , null, oInstanciaOTD
                                            , new object[] { oValor }
                                            );

            }

            //devolvemos el resultado
            return (T)oInstanciaOTD;

        }


        /// <summary>
        /// Devuelve el diccionario de pares Nombre campo 
        /// físico - Propiedad de la instancia
        /// </summary>
        public Dictionary<string, PropertyInfo> Get_dic_propiedades()
        {
            return __dicPropiedades;
        }

        /// <summary>
        /// Devuelve el diccionario de pares Nombre campo 
        /// Fisico - Campo de la instancia
        /// </summary>
        public Dictionary<string, Campo> Get_dic_campos()
        {
            return __dicCampos;
        }


        #endregion



        #region SOBREESCRITOS

        /// <summary>
        /// Devuelve la representacion de la clase
        /// </summary>
        /// <returns>Representacion de la Clase</returns>
        public override string ToString()
        {
            return string.Format("<{0} - [{1}]>", _nId, _cDescripcion.ToUpper());
        }

        /// <summary>
        /// Metodo de comparacion de instancias
        /// </summary>
        /// <param name="obj">Instancia de la clase OTDbase contra la cual comparar</param>
        /// <returns>true si los objetos tienen el mismo Id</returns>
        public override bool Equals(object obj)
        {
            //evaluamos si el parametro es del mismo tipo
            if (!obj.GetType().Equals(GetType())) return false;

            //luego si es el mismo ID
            return _nId.Equals(((OTDbase)obj)._nId);
        }


        #endregion



        #region GENERADORES CODIGO DML

        /// <summary>
        /// Ejecuta el seteo de los datos de la TABLA referenciada
        /// </summary>
        /// <typeparam name="T">Tipo de OTDbase extendido</typeparam>
        protected void _Set_campos<T>()
        {
            //si aun no esta instanciado
            if (__dicCampos == null)
            {
                //instanciamos los nuevos diccionarios
                __dicCampos = new Dictionary<string, Campo>();
                __dicPropiedades = new Dictionary<string, PropertyInfo>();

                //tomamos el tipo de la instancia
                Type tTipoOTD = typeof(T);

                //tomamos los campos de la clase
                PropertyInfo[] aPropiedades = tTipoOTD.GetProperties(BindingFlags.GetProperty
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
                            if(oCampo.Indice == 0) oCampo.Indice = nIdx;

                            //le asignamos su tipo
                            oCampo.Tipo = oPropiedad.PropertyType;

                            //si el campo NO esta en en el dicionario ya
                            if (!__dicCampos.ContainsKey(oCampo.Nombre))
                                //tomamos la instancia correspondiente y la agregamos al diccionario
                                __dicCampos.Add(oCampo.Nombre, oCampo);

                            //incrementamos el contador
                            nIdx+= 1;
                        }
                    }
                }
            }
        }
        
        /// <summary>
        /// Devuelve el diccionario de pares Parametro-Valor para las operaciones
        /// DML
        /// </summary>
        /// <param name="bIncluirNulos">Si se incluyen o no los pares con valores nulos</param>
        /// <returns>Diccionario de pares [NombrePropiedad, oValor]</returns>
        public Dictionary<string, object> ParametrosDML(bool bIncluirNulos)
        {
            var dicParametros = new Dictionary<string, object>();

            //tomamos el tipo de la instancia
            Type tTipoOtTD = this.GetType();

            //recorremos los metodos de la clase actual
            foreach (var cCampo in __dicPropiedades.Keys)
            {
                //llamamos al metodo setter actual
                var oValor = tTipoOtTD.InvokeMember(__dicPropiedades[cCampo].Name
                                                    , BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.Public
                                                    , null, this, new object[]{}
                                                    );

                //si elvalor no es nulo
                if (oValor != null)
                    //recuperamos el valor de la propiedad y agregamos el par de parametros
                    dicParametros.Add(__dicCampos[cCampo].Nombre.ToLower(), oValor);
                //sino, si el valor es nulo Y se requieren
                else if(bIncluirNulos)
                    //recuperamos el valor de la propiedad y agregamos el par de parametros
                    dicParametros.Add(__dicCampos[cCampo].Nombre.ToLower(), oValor);
               
            }

            //devolvemos el diccionario
            return dicParametros;
        }

        /// <summary>
        /// Devuelve una cadena con los nombres de los campos
        /// CAMPO0, CAMPO1, CAMPO2, ...
        /// </summary>
        public string ListaDeCampos()
        {
            //sino la escribimos
            StringBuilder sbLista = new StringBuilder();

            //recorremos los campos
            foreach (Campo oCampo in this.Get_dic_campos().Values)
            {
                if (sbLista.ToString() == string.Empty) sbLista.Append(oCampo.Nombre);
                else sbLista.Append(", ").Append(oCampo.Nombre);
            }

            //devolvemos el resultado
            return sbLista.ToString();
        }


        #endregion

    }
}
