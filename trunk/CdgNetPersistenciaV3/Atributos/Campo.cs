using System;
using System.Collections.Generic;

using System.Text;

/**
 * Autor: Cristian Daniel Garay Sosa
 * Fecha :  27/03/2012 (original)
 * Comentarios:
 *          lo que es una anotacion en Java, sirve para generar metadatos, en este caso relacionar
 *          un setter con el campo del cual recogera los datos
 * 
 * */

namespace CdgNetPersistenciaV3.Atributos
{
    /**[System.AttributeUsage(System.AttributeTargets.Method
                            , AllowMultiple = false)
    ]**/

    /// <summary>
    /// Atributo de relacion SETTER de OTD - Campo de Tabla Fisica
    /// </summary>
    /// <see cref="http://msdn.microsoft.com/es-es/library/sw480ze8%28v=vs.80%29.aspx"/>
    [System.AttributeUsage(System.AttributeTargets.Property
                            , AllowMultiple = false)
    ]
    public class Campo : System.Attribute
    {

        #region CAMPOS(Fields)

        const string NOMBRE_CLASE = "Campo";

        /// <summary>
        /// Devuelve o establece el nombre del Campo
        /// </summary>
        private string __cNombre;

        /// <summary>
        /// Devuelve o establece el marcador de campo PK
        /// </summary>
        public bool Identificador = false;

        /// <summary>
        /// Devuelve o establece la posicion del campo
        /// </summary>
        public int Indice = 0;

        /// <summary>
        /// Devuelve o establece el marcador de permision de valores NULL 
        /// </summary>
        public bool Nulable = false;

        /// <summary>
        /// Devuelve o establece el tipo de dato del campo
        /// </summary>
        public Type Tipo;

        /// <summary>
        /// Devuelve o establece el marcador de campo autogenerado por el SGBD
        /// </summary>
        public bool AutoGenerado = false;

        
        #endregion



        #region CONSTRUCTORES

        /// <summary>
        /// Contructor de la Clase
        /// </summary>
        /// <param name="cNombreCampo">Nombre del Campo de Tabla al que hace referencia la Propiedad</param>
        public Campo(string cNombreCampo)
        {
            __cNombre = cNombreCampo;
        }

        #endregion



        #region GETTERS Y SETTERS

        /// <summary>
        /// Devuelve el nombre del campo de la tabla
        /// </summary>
        public string Nombre
        {
            get {return __cNombre;}
        }
        

        #endregion 



        #region SOBREESCRITOS

        /// <summary>
        /// Devuelve la representacion de la instancia
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("<{0} - [{1}({2})]>", NOMBRE_CLASE, __cNombre, Tipo);
        }

        #endregion

    }
}
