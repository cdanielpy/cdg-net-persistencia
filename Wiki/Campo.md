La clase Campo
==============

Esta clase extiende de la clase `System.Attribute` y mediante su uso lo que se
hace es enlazar una propiedad, de cualquier instancia de la clase [OTDbase][1],
a un campo físico de una tabla.-

[1]: <https://github.com/cdanielpy/cdg-net-persistencia/blob/master/Wiki/OTDbase.md>

*Obs.:* Todos las propiedades de una instancia de [OTDbase] deben hacer
referencia sólo a los campos de la misma tabla que representa el OTD donde se
aplica.-

 

Constructores
-------------

### Campo(cNombreCampo):

**cNombreCampo:** (`System.String`) Nombre del campo físico de la tabla al cual
hace referencia la propiedad a la que se aplica.-

*Obs:* Dependiendo del SGBD y/o el SO, los nombres deben respetar mayúsculas y
minúsculas.-

 

Campos
------

### Privados

**\_\_cNombre:** (`System.String`) Almacena el nombre del Campo fisico de la
tabla.-

### Públicos

**NOMBRE\_CLASE:** (`System.String`) *Estático*. Almacena el nombre de la clase
en forma de simple cadena.-

**Identificador:** (`System.Boolean`) Devuelve o establece el marcador de campo
PK o Clave Primaria . Valor por defecto: `false`.-

**Indice:** (`System.Int32`) Devuelve o establece la posición del campo respecto
de los demas campos. Si no se indica, se considera la posición de declaración
del accesor dentro del OTD. Valor por defecto: 0.-

**Nulable:** (`System.Boolean`) Devuelve o establece el marcador que indica que
el campo acepta vsalores *NULL*. Valor por defecto: `true`.-

*Obs:* Si se establece la propiedad *Identificador* como `true`, se asume que no
acepta valores nulos.-

**Tipo:** (`System.Type`) Devuelve o establece el tipo de dato del campo.-

**AutoGenerado:** (`System.Boolean`) Devuelve o establece el marcador de campo
autogenerado por el SGBD, es decir, valores de campo autoincrementales o
calculados y que no aceptan valores asignados en forma externa. Valor por
defecto: `false`.-

 

Propiedades
-----------

### Públicas

**Nombre:** (`System.String`) Devuelve el nombre del campo físico al cual
representa.-

 

Métodos
-------

### Públicos

**ToString():** (`System.String`) *Sobreescrito*. Devuelve la representacion de
cadena de la instancia.-

 

Ejemplos
--------

Ver definición del OTD de [Personas]
