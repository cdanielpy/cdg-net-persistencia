# La clase ADMbase #

Esta es una clase abstracta que debe ser extendida por las clases que harán el trabajo de centralizar las acciones de manipulación de datos de cada tabla física del SGBD, es decir, un _DAO_. Se encarga de establecer las operaciones básicas de tipo _CRUD_, así como administrar los nombres de campos, las sentencias a ejecutar y los métodos de paso de valores de resultados a propiedades de instancias de sus _VOs_ (o _DTOs_).-

**Ésta es la clase central de toda la librería ya que aquí es donde se centran los métodos de 'automatización' de comandos DML y el tratamiento de los resultados**.


# Constructores #
**ADMbase(**`oConectorBase`**):**
  * _oConectorBase:_ (_[ConectorBase](http://code.google.com/p/cdg-net-persistencia/wiki/CdgNetPersistenciaV3_ClasesBases_ConectorBase)_) Instancia de extendida de _ConectorBase_, que sería la útileria de interacción con el SGBD espécifico.-

# Campos #
## Privados ##
  * **`__`SELECT\_SQL:** (`System.String`) Constante. Contiene la cadena base de selección de registros.-
  * **`__`INSERT\_SQL:** (`System.String`) Constante. Contiene la cadena base de inserción de registros.-
  * **`__`UPDATE\_SQL:** (`System.String`) Constante. Contiene la cadena base de actualización de registros.-
  * **`__`DELETE\_SQL:** (`System.String`) Constante. Contiene la cadena base de eliminación de registros.-
  * **`__`cSelect\_sql:** (`System.String`) Almacena la cadena de selección de registros de la tabla.-
  * **`__`cInsert\_sql:** (`System.String`) Almacena la cadena de inserción de registros de la tabla.-
  * **`__`cUpdate\_sql:** (`System.String`) Almacena la cadena de actualizacion de registros de la tabla.-
  * **`__`cDelete\_sql:** (`System.String`) Almacena la cadena de eliminación de registros de la tabla.-
  * **`__`cWhereSugerido:** (`System.String`) Almacena la cadena base sugerida para filtrado de registros de la tabla.-
  * **`__`cNombreTabla:** (`System.String`) Almacena el nombre físico de la tabla.-


## Protegidos ##
  * **`_`tTipoOTD:** (`System.Type`) Almacena el tipo de de la clase extendida de OTDbase.-
  * **`_`cListaCamposTabla:** (`System.String`) Almacena la lista de nombres de campos físicos de la tabla en una lista separada por comas.-

## Públicos ##
  * **NOMBRE\_CLASE:** (`System.String`) Estático. Almacena el nombre de la clase en forma de simple cadena.-
  * **MARCADOR\_PARAMETRO\_ESTANDAR:** (`System.Char`) Constante. Marcador estándar de nombres de parámetros de comandos SQL.-
  * **cMarcaParametro:** (`System.Char`) Almacena el marcador de parámetros SQL.-
  * **`_`oConector:** (_[ConectorBase](http://code.google.com/p/cdg-net-persistencia/wiki/CdgNetPersistenciaV3_ClasesBases_ConectorBase)_) Almacena la instancia de utilería de conexión a la base de datos.-

# Propiedades #
## Públicas ##
  * **NombreTabla:** (`System.String`) Devuelve el nombre de la tabla administrada.-
  * **Select\_sql:** (`System.String`) Virtual. Devuelve o establece el comando de selección de datos de la tabla.-
  * **Insert\_sql:** (`System.String`) Virtual. Devuelve o establece el comando de inserción de datos a la tabla.-
  * **Update\_sql:** (`System.String`) Virtual. Devuelve o establece el comando de actualización de datos de la tabla.-
  * **Delete\_sql:** (`System.String`) Virtual. Devuelve o establece el comando de aliminación de datos de la tabla
  * **WhereSugerido:** (`System.String`) Virtual. Devuelve la condición de filtrado de registros sugerida en base a la estructura especificada de la tabla.-

# Métodos #
## Privados ##
**`__`SetInsertSql():** (`System.Void`) Establece la sentencia sql base de inserción de registros en la tabla administrada.-

**`__`SetUpdateSql():** (`System.Void`) Establece la sentencia sql base de actualización de registros en la tabla administrada.-

**`__`SetWhereSugerido():** (`System.Void`) Establece las condiciones de filtrado de registros para operaciones DML en base a la estructura especificada de la tabla.-


## Protegidos ##
**`_`lSet\_registros`<`T`>`(**`aFilasParam`**):** (`List<T>`) Ejecuta la instanciación y asignación del OTD administrado por la clase.-
  * _T:_ (`Tipo`) Clase de OTDbase extendida a los que seran 'convertidas' las filas del _DataTable_.-
  * _aFilasParam:_ (`DataRowCollection`) Arreglo de filas de un DataTable.-

**`_`Set\_tabla`<`T`>`():** (`System.Void`) Establece los datos de la estructura física de la tabla en base a la instancia del OTDbase parámetro.-
  * _T:_ (`Tipo`) Clase de OTDbase extendida que contiene los datos de la tabla física.-

**`_`SetListaCamposTabla():** (`System.Void`) Devuelve una cadena con los nombres de los campos con alias de la incluído.-

**`_`SetInsertSql`<`T`>`():** (`System.Void`) Establece la sentencia sql base de inserción de registros en la tabla administrada.-
  * _T:_ (`Tipo`) Clase de OTDbase extendida que contiene los datos de la tabla física.-

**`_`SetUpdateSql():** (`System.Void`) Establece la sentencia sql base de actualización de registros en la tabla administrada.-
  * _T:_ (`Tipo`) Clase de OTDbase extendida que contiene los datos de la tabla física.-

**`_`SetWhereSugerido():** (`System.Void`) Establece las condiciones de filtrado de registros para operaciones DML en base a la estructura especificada de la tabla.-
  * _T:_ (`Tipo`) Clase de OTDbase extendida que contiene los datos de la tabla física.-



## Públicos ##
**ToString():** (`System.String`) Sobreescrito. Devuelve la representacion de cadena de la clase.-

**lAgregar(**`oOTDbase`**):** (`List<System.Object>`) Abstracto. Inserta un nuevo registro en la tabla a partir de la instancia parámetro.-

  * _oOTDbase:_ ([OTDBase](http://code.google.com/p/cdg-net-persistencia/wiki/CdgNetPersistenciaV3_ClasesBases_OTDbase)) Instancia de la clase extendida de OTDbase de la cual se obtienen los datos del registro a insertar.-

**lAgregar`<`T`>`(**`OTDBase`**):** (`List<System.Object>`) Virtual. Ídem a la versión abstracta, pero se debe especificar la clase extendida de _OTDbase_ de la cual se tomarán los datos a insertar.-

**lActualizar(**`OTDBase`**):** (`List<System.Object>`) Abstracto. Actualiza el registro en la tabla a partir de la instancia parámetro. La instancia, a su vez, representa al registro a actualizar.-

**lActualizar`<`T`>`(**`OTDBase`**):** (`List<System.Object>`) Virtual. Ídem a la versión abstracta, pero se debe especificar la clase extendida de _OTDbase_ de la cual se tomarán los datos a eliminar.-

**lEliminar(**`OTDBase`**):** (`List<System.Object>`) Abstracto. Elimina el registro de la tabla representado por la instancia parámetro. La instancia, a su vez, representa al registro a eliminar-

**lEliminar`<`T`>`(**`OTDBase`**):** (`List<System.Object>`) Virtual. Ídem a la versión abstracta, pero se debe especificar la clase extendida de _OTDbase_ de la cual se tomarán los datos a eliminar.-

**lGet\_elemento(**`OTDBase`**):** (`List<System.Object>`) Abstracto. Obtiene una única instancia que coincide con el ID de la instancia parámetro, o, en su defecto, por los demas valores de atributos no nulos.-

**lGet\_elemento`<`T`>`(**`OTDBase`**):** (`List<System.Object>`) Virtual. Ídem a la versión abstracta, pero se debe especificar la clase extendida de _OTDbase_ de la cual se tomarán los datos a comparar.-

**lGet\_elementos(**`OTDBase`**):** (`List<System.Object>`) Abstracto. Obtiene una lista de instancias que satisfacen la condicion de filtrado desde la base de datos.-

**lGet\_elementos`<`T`>`(**`OTDBase`**):** (`List<System.Object>`) Virtual. Virtual. Ídem a la versión abstracta, pero se debe especificar la clase extendida de _OTDbase_ a la cual se transformarán los datos a recuperar.-