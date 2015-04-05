La clase OTDbase
================

Esta es una clase abstracta que debe ser extendida por los DTOs. Se encarga de
encapsular todas las operaciones que permiten que las instancias operen de un
modo parecido a los Java Beans.-

Constructores
-------------

### OTDbase():

Constructor por defecto.-

### OTDbase(nIdParam, cDescripcionParam):

**nIdParam:** (`System.Int64`) Identificador de la instancia. Normalmente éste
es el valor del campo PK del registro.-

**cDescripcionParam:** (`System.String`) Descripción de la instancia. Puede ser
cualquier valor de cadena, puede ser muy útil para realizar consultas cuando no
se tiene el valor del identificador de la instacia.-

Campos
------

### Privados

**\_\_dicCampos:** (`Dictionary<System.String, Campo>`) Dicionario para
relacionar los campos de la instancia a los Campos de la tabla.-

**\_\_dicPropiedades:** (`Dictionary<System.String, PropertyInfo>`) Diccionario
para relacionar campos de tabla física y las Propiedades referenciando a los
mismos por medio de Campos.-

### Públicos

**NOMBRE\_CLASE:** (`System.String`) *Estático*. Almacena el nombre de la clase
en forma de simple cadena.-

### Protegigas

**\_nId:** (`System.Int64`) Almacena el valor del identificador de la instancia.
Normalmente debería ser el valor del campo PK de la tabla.-

**\_cDescripcion:** (`System.String`) Almacena la descripción de la instancia.
Se sugiere referenciarlo al campo de cadena de caracteres más relevante de la
tabla-

Propiedades
-----------

### Públicas

**Id:** (`System.Int64`) Devuelve o establece el valor del identificador de la
clase.-

**Descripcion:** (`System.String`) Devuelve o establece la descripción de la
instancia.-

 

Métodos
-------

### Públicos

**Get\_clon\<T\>()**: (`T`) Crea y devuelve un clon de la instancia actual. `T`
es la clase destino de la instancia creada.-

    Obs: `T` siempre debería ser del mismo tipo que la clase del OTD extendida,
    porque de otro modo, sólo se clonarán los valores de prodiedades que sean
    comunes a ambas clases.-

**ToString()**: (`System.String`) *Sobreescrito*. Devuelve la representación de
cadena de la instancia.-

**Equals(obj):** (`System.Boolean`) *Sobreescrito*. Método de comparación de
instancias. En este caso evalúa si son iguales por el valor del campo Id.-

**obj**: (`System.Object`) Instancia contra la cual se desea realizar la
comparación.-

 

Ejemplos
--------

Ver definición del OTD de [Personas][1]

[1]: <https://github.com/cdanielpy/cdg-net-persistencia/blob/master/Testing/PersonaOTD.cs>
