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

namespace CdgNetPersistenciaV3_5.Atributos
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
        /// Almacena el nombre del Campo fisico de la tabla
        /// </summary>
        private string __cNombre;

        /// <summary>
        /// Devuelve o establece el marcador de campo PK
        /// </summary>
        public bool Identificador = false;

        /// <summary>
        /// Devuelve o establece la posicion del campo respecto
        /// de los demas campos. Si no se indica, se considera
        /// la posicion de declaracion del accesor dentro del
        /// OTD
        /// </summary>
        public int Indice = 0;

        /// <summary>
        /// Devuelve o establece el marcador de permision de valores NULL
        /// para el campo
        /// </summary>
        public bool Nulable = false;

        /// <summary>
        /// Devuelve o establece el tipo de dato del campo
        /// </summary>
        public Type Tipo;

        /// <summary>
        /// Devuelve o establece el marcador de campo autogenerado por el SGBD,
        /// es decir, valores de campo autoincrementales o calculados y que 
        /// no aceptan valores asignados
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
        /// Devuelve el nombre del campo físico de la tabla
        /// </summary>
        public string Nombre
        {
            get {return __cNombre;}
        }
        

        #endregion 



        #region SOBREESCRITOS

        /// <summary>
        /// Devuelve la representacion de de cadena de la instancia
        /// </summary>
        /// <returns>Cadena de representacion de la instancia de esta clase</returns>
        public override string ToString()
        {
            return string.Format("<{0} - [{1}({2})]>", NOMBRE_CLASE, __cNombre, Tipo);
        }

        #endregion

    }
}
