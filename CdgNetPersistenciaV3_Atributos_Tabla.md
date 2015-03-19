# La clase Tabla #

Esta clase extiende de la clase _System.Attribute_ y mediante su uso lo que se hace es indicar, a la clase derivada de de la clase _OTDbase_ donde se aplica, el nombre físico de la tabla en el SGBD y el nombre del mismo, a fin seguir las notaciones de comandos especificos del sistema, como ser los nombres de parametros de procedimientos almacenados.-

`  `_**Obs.:**_ Dependiendo del SGBD y el SO, los nombres deben respetar mayúsculas y minúsculas.-


# Constructores #
`  `**Tabla(**_`cNombreTabla, eNombreSGBD`_**):**
  * _cNombreTabla:_ (`System.String`) Nombre de la tabla física a la que referencia la instancia.-
  * _eNombreSGBD:_ (`Tabla.SGBD`) Nombre del SGBD para establecer los tipos de consultas parametrizadas.-


# Campos #
## Privados ##
  * **cNombre:** (`System.String`) Almacena el nombre de la Tabla física de la tabla.-
  * **eTipo:** (`Tabla.SGBD`) Almacena el enumerador que identifica al SGBD.-

## Públicos ##
  * **NOMBRE\_CLASE:** (`System.String`) Estático. Almacena el nombre de la clase en forma de simple cadena.-
  * **SGBD:** (`enum`) Enumerador que almacena los identificadores de los diferentes SGBD a con los que se puede implementar la libreria proyecto.-


# Propiedades #
## Públicas ##
  * **Nombre:** (`System.String`) Devuelve el nombre de la tabla física.-
  * **TipoSGBD:** (`Tabla.SGBD`) Devuelve el tipo de SGBD donde se aloja la tabla.-


# Métodos #
## Públicos ##
  * **ToString():** (`System.String`) Sobreescrito. Devuelve la representacion de de cadena de la instancia.-

# Ejemplos #
Ver definición de _VO_ de [Personas](http://code.google.com/p/cdg-net-persistencia/wiki/CdgNetPersistenciaV3_Atributos_Campo):