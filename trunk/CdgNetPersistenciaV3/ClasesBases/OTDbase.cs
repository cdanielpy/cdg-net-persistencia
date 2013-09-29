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

        /// <summary>
        /// Devuelve o establece el Identificador de la instancia
        /// </summary>
        public virtual long Id {
            get {
                return _nId;
            }
            set {
                _nId = value;
            }
        }

        /// <summary>
        /// Devuelve o establece la Descripcion de la instancia
        /// </summary>
        public virtual string Descripcion
        {
            get
            {
                return _cDescripcion;
            }
            set
            {
                _cDescripcion = value;
            }
        }

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

    }
}
